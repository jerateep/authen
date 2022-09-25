
using auth.models.DB;
using auth.models.Dto;
using System.Security.Claims;

namespace auth.jwt.Interface
{
    public interface ITokenService
    {
        string CreateToken(TBL_User _user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
