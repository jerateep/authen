using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace auth.hmac.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;


        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, Interface.IAuthentication _authenService)
        {
            string SecretKey = "@uthAp1_test!!";
            var hmacHeader = context.Request.Headers["hmac"].FirstOrDefault()?.Split(" ").Last();
            var timestampHeader = context.Request.Headers["timestamp"].FirstOrDefault()?.Split(" ").Last();
            var ServiceName = context.Request.Headers["sercice_name"].FirstOrDefault()?.Split(" ").Last();
            string strHmac = new Extensions.HmacExtensions().HMAC_SHA256(Convert.ToInt32(timestampHeader), ServiceName, SecretKey);
            if (strHmac == hmacHeader)
            {
                bool result = _authenService.CheckHmacDB(hmacHeader, Convert.ToInt32(timestampHeader), ServiceName);
                if (result)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, ServiceName)
                    };
                    var appIdentity = new ClaimsIdentity(claims);
                    context.User.AddIdentity(appIdentity);
                }
            }
            await _next(context);
        }

    }
}
