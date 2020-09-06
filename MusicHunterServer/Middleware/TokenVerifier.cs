using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicHunterServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MusicHunterServer.Middleware
{
    public class TokenVerifier
    {
        private List<string> WhiteListUrls;
        private readonly RequestDelegate _next;
        private readonly IOptions<AppSettings> _appSettings;
        public TokenVerifier(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            this._next = next;
            this._appSettings = appSettings;
            //Urls white list for token verifier
            this.WhiteListUrls = new List<string>()
            {
                "/api/public",
                "/messenger",
                "/notifications",
                "/images",
                "/music"
            };
            //---------------------------------------

        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (IsUrlInWhiteList(context)) {
                await _next.Invoke(context);
            }
            else
            {
                var token = context.Request.Headers["token"];
                if (!ValidateCurrentToken(token))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Invalid token" }));

                }
                else
                {
                    await _next.Invoke(context);
                }
            }
            
        }


        public bool ValidateCurrentToken(string token)
        {
            var mySecret = _appSettings.Value.Secret;
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateActor = false,
                    ValidateTokenReplay = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = mySecurityKey,


                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool IsUrlInWhiteList(HttpContext context)
        {
            foreach (var url in this.WhiteListUrls)
            {
                if (context.Request.Path.StartsWithSegments(url)) {
                    return true;
                }
            }
            return false;
        }
    }
}
