using auth.basic.Interface;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace auth.basic.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader))
                {
                    var authHeaderParse = AuthenticationHeaderValue.Parse(authHeader);
                    var credentialBytes = Convert.FromBase64String(authHeaderParse.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                    var username = credentials[0];
                    var password = credentials[1];

                    var user = await userService.Authenticate(username, password);
                    if (!string.IsNullOrEmpty(user.Username))
                    {
                       var claims = new List<Claim>
                       {
                                new Claim(ClaimTypes.Name, user.Username)
                       };
                       var appIdentity = new ClaimsIdentity(claims);
                       context.User.AddIdentity(appIdentity);
                    }
                }
            }
            catch
            {
  
            }

            await _next(context);
        }
    }
}
