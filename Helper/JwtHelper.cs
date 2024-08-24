using JwtAuth.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuth.Helper
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            this._config = config;
        }
        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]??
                throw new Exception("Jwt key is missing"))
                );
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            List<Claim> claims = new List<Claim>(){
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Sid, user.UserId.ToString()),
                                new Claim(ClaimTypes.Name, user.Email),
                                new Claim(ClaimTypes.Role, user.Role),
                                new Claim("Date", DateTime.Now.ToString()),
                                };

            var token = new JwtSecurityToken(
                  _config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims.ToArray(),
                  notBefore:DateTime.Now,
                  expires:DateTime.Now.AddMinutes(10),
                  signingCredentials: credential
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false 
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
