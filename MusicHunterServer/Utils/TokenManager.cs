using Microsoft.IdentityModel.Tokens;
using MusicHunterServer.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHunterServer.Utils
{
    public class TokenManager
    {
        private string secretKey;
        public TokenManager(string secretKey)
        {
            this.secretKey = secretKey;
        }
        public string createToken(AuthenticateRequest request)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(credentials);
            var now = DateTime.UtcNow;
            var payload = new JwtPayload
            {
                {"Email", request.Email},
                {"iat", now},
            };
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(secToken);
            return tokenString;
        }
        public bool ValidateToken(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.secretKey));
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


        public bool ValidateToken(string token, out SecurityToken validatedToken)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.secretKey));
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
                }, out validatedToken);

            }
            catch
            {
                validatedToken = null;
                return false;
            }
            return true;
        }


        public string ValidateAndGetEmail(string token)
        {
            SecurityToken validatedToken;
            if (ValidateToken(token, out validatedToken))
            {
                string email = ((JwtSecurityToken)validatedToken).Payload["Email"].ToString();
                return email;
            }
            return null;
        }
    }
}
