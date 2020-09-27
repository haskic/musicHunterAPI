using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicHunterServer.Models;
using MusicHunterServer.Utils;
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
        private readonly TokenManager _tokenManager;
        public TokenVerifier(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            this._next = next;
            this._appSettings = appSettings;
            this._tokenManager = new TokenManager(_appSettings.Value.Secret);
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
            if (IsUrlInWhiteList(context))
            {
                await _next.Invoke(context);
            }
            else
            {
                var token = context.Request.Headers["token"];
                if (!this._tokenManager.ValidateToken(token))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "[Middleware] Invalid token" }));
                }
                else
                {
                    await _next.Invoke(context);
                }
            }
        }



        private bool IsUrlInWhiteList(HttpContext context)
        {
            foreach (var url in this.WhiteListUrls)
            {
                if (context.Request.Path.StartsWithSegments(url))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
