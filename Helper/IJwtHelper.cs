using System.Security.Claims;
using JwtAuth.Entities;

namespace JwtAuth.Helper
{
    public interface IJwtHelper
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
