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
using Newtonsoft.Json.Serialization;

namespace MusicHunterServer.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<Program> _logger;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly TokenManager _tokenManeger;
        private AppDbContext _dbContext;
        private readonly DefaultContractResolver contractResolver;
        public AuthController(ILogger<Program> logger, IOptions<AppSettings> appSettings, AppDbContext dbContext)
        {
            _logger = logger;
            _appSettings = appSettings;
            this._dbContext = dbContext;
            this._tokenManeger = new TokenManager(appSettings.Value.Secret);
            this.contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        [Route("/api/public/login")]
        [HttpPost]
        public string Login([FromBody] AuthenticateRequest loginRequest)
        {
            _logger.LogInformation($"LOGIN REQUEST FROM {loginRequest.Email}");

            var result = _dbContext.Users.Where(user => user.Email == loginRequest.Email && user.Password == loginRequest.Password).FirstOrDefault();

            if (result == null)
            {
                return JsonConvert.SerializeObject(new { message = "User was not founded", status = false });
            }
            string tokenString = _tokenManeger.createToken(loginRequest);
            return JsonConvert.SerializeObject(new { message =  "login successed", token = tokenString, userHash = result.Hash,  });

        }

        [Route("/api/public/registration")]
        [HttpPost]
        public async Task<string> RegistrationUser(User user)
        {
            this._logger.LogInformation($"Registration request from user: {user.Email}");
            this._logger.LogInformation($"Nickname: {user.Nickname}");

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.Hash = Hasher.GetHashString(user.Email + user.CreatedAt.ToString());
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return JsonConvert.SerializeObject(new { message = $"Registration {user.Email} successed", status = true });
        }


        [Route("api/public/googleVerify")]
        [HttpPost]
        public async Task<string> GoogleTokenVerify(GoogleToken token)
        {
            string tokenString = "";
            try
            {
                var result = await GoogleJsonWebSignature.ValidateAsync(token.TokenId);
                string UserHash = "";
                var userInDb = _dbContext.Users.Where(user => user.Email == result.Email).FirstOrDefault();

                if (userInDb == null)
                {
                    User newUser = new User() { Email = result.Email, IsBlocked = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                    newUser.Hash = Hasher.GetHashString(result.Email + newUser.CreatedAt.ToString());
                    UserHash = newUser.Hash;
                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    UserHash = userInDb.Hash;
                }

                AuthenticateRequest authRequest = new AuthenticateRequest()
                {
                    Email = result.Email
                };
                tokenString = _tokenManeger.createToken(authRequest);
                return JsonConvert.SerializeObject(new { message = "Google token obj veryfy successed", status = true, token = tokenString, userHash = UserHash });

            }
            catch (Exception e)
            {
                _logger.LogInformation("GoogleValidate: Invalid GoogleToken");
                return JsonConvert.SerializeObject(new { message = "Google token obj validation error", status = false });

            }
        }
    }
}