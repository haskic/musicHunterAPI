using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Google.Apis.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicHunterServer.Data;
using MusicHunterServer.Models;
using MusicHunterServer.Utils;
using Newtonsoft.Json;



namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<Program> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly TokenManager _tokenManeger;
        private AppDbContext _dbContext;

        public AuthController(ILogger<Program> logger, IOptions<AppSettings> appSettings, AppDbContext dbContext)
        {
            _logger = logger;
            _appSettings = appSettings;
            this._dbContext = dbContext;
            this._tokenManeger = new TokenManager(appSettings.Value.Secret);
        }

        [Route("/api/public/login")]
        [HttpPost]
        public string Login([FromBody] AuthenticateRequest loginRequest)
        {
            _logger.LogInformation("POST RESPONSE /login");

            var result = _dbContext.Users.Where(user => user.Email == loginRequest.Email && user.Password == loginRequest.Password).FirstOrDefault();

            if (result == null)
            {
                _logger.LogWarning("Login denied | user was not founded");
                return JsonConvert.SerializeObject(new { message = "User was not founded", status = false });
            }

            string tokenString = _tokenManeger.createToken(loginRequest);
            return JsonConvert.SerializeObject(new { message =  "login successed", token = tokenString });

        }

        [Route("/api/public/registration")]
        [HttpPost]
        public async Task<string> RegistrationUser(User user)
        {
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.Hash = Hasher.GetHashString(user.Email + user.Password + user.CreatedAt.ToString()); ;
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = "Registratinos successed", status = true });
        }


        [Route("api/public/googleVerify")]
        [HttpPost]
        public async Task<string> GoogleTokenVerify(GoogleToken token)
        {
            string tokenString = "";
            try
            {
                var result = await GoogleJsonWebSignature.ValidateAsync(token.TokenId);
                _logger.LogInformation("Google token received with email: " + result.Email);

                var isUserInDb = _dbContext.Users.Where(user => user.Email == result.Email).FirstOrDefault();

                if (isUserInDb == null)
                {
                    User newUser = new User() { Email = result.Email, IsBlocked = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                }

                AuthenticateRequest authRequest = new AuthenticateRequest()
                {
                    Email = result.Email
                };
                tokenString = _tokenManeger.createToken(authRequest);

            }
            catch (Exception e)
            {
                _logger.LogInformation("GoogleValidate: Invalid GoogleToken");
                return JsonConvert.SerializeObject(new { message = "Google token obj validation error", status = false });

            }
            return JsonConvert.SerializeObject(new { message = "Google token obj veryfy successed", status = true, token = tokenString});
        }

        [Route("api/testroute")]
        [HttpPost]
        public async Task<string> TestRouter()
        {


            return JsonConvert.SerializeObject(new { message = "test success" });
        }

    }
}