using auth.jwt.Interface;
using auth.jwt.Services;
using auth.models.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace auth.jwt.Extensions
{
    public static class IdentityServiceExtensions
    {

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            Jwt jwt = new Jwt();
            string secret_key = jwt.secret_key;
            int expires = jwt.expires;
            string Issuer = jwt.Issuer;
            string Audience = jwt.Audience;
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(option =>
                            {
                                option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_key)),
                                    ValidIssuer = Issuer,
                                    ValidAudience = Audience,
                                    ClockSkew = TimeSpan.Zero
                                };
                            });
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
