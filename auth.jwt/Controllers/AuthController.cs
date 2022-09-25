using auth.jwt.Interface;
using auth.models.Data;
using auth.models.DB;
using auth.models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace auth.jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _db;
        private readonly ITokenService _tokenService;
        public AuthController(DataContext db, ITokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<JwtDto>> Login(LoginDto _login)
        {
            var user = await _db.TBL_User.SingleOrDefaultAsync(o => o.Username == _login.username);

            if (user == null)
            {
                return Unauthorized("Invalid Username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var comoutedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_login.password));
            for (int i = 0; i < comoutedHash.Length; i++)
            {
                if (comoutedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password");
                }
            }
            string Token = _tokenService.CreateToken(user);
            string RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
            _db.SaveChanges();
            return new JwtDto
            {
                username = user.Username,
                Token = Token,
                RefreshToken = RefreshToken
            };
        }

        [HttpPost("Register")]
        public async Task<ActionResult<JwtDto>> Register(RegisterDto _register)
        {
            if (await UserExists(_register.userName))
            {
                return BadRequest("Username is token");
            }
            using var hmac = new HMACSHA512();
            var user = new TBL_User
            {
                Username = _register.userName,
                Firstname = _register.Firstname,
                Lastname = _register.Lastname,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_register.password)),
                PasswordSalt = hmac.Key,
                RefreshToken = _tokenService.GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(1)

        };
            _db.TBL_User.Add(user);
            await _db.SaveChangesAsync();
            return new JwtDto
            {
                username = user.Username,
                Token = _tokenService.CreateToken(user),
                RefreshToken = user.RefreshToken
            };
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<JwtDto>> RefreshToken(TokenDto _token)
        {
            if (_token is null)
            {
                return Unauthorized("Invalid client request");
            }
            var principal = _tokenService.GetPrincipalFromExpiredToken(_token.Token);
            var username = principal.Claims.Where(o => o.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value; 
            var user = await _db.TBL_User.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || user.RefreshToken != _token.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }
            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _db.SaveChanges();
            return new JwtDto
            {
                username = user.Username,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        private async Task<bool> UserExists(string userName)
        {
            return await _db.TBL_User.AnyAsync(o => o.Username.ToLower() == userName.ToLower());
        }
    }
}
