
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using auth.models.Dto;
using auth.jwt.Interface;
using auth.models.Internal;
using auth.models.DB;

namespace auth.jwt.Services
{
    public class TokenService : ITokenService
    {
        public TokenService()
        {

        }
        public string CreateToken(TBL_User _user)
        {
            Jwt jwt = new Jwt();
            string secret_key = jwt.secret_key;
            int expires = jwt.expires;
            string Issuer = jwt.Issuer;
            string Audience = jwt.Audience;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_key));
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, _user.Firstname),
                new Claim(JwtRegisteredClaimNames.FamilyName, _user.Lastname),
                new Claim(JwtRegisteredClaimNames.NameId, _user.Username)
            };
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            DateTime dTExpires = DateTime.Now.AddMinutes(expires);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),           
                Expires = dTExpires,
                SigningCredentials = creds,
                Audience = Audience,
                Issuer = Issuer,      
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            Jwt jwt = new Jwt();
            string secret_key = jwt.secret_key;
            //int expires = jwt.expires;
            string Issuer = jwt.Issuer;
            string Audience = jwt.Audience;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_key)),
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
