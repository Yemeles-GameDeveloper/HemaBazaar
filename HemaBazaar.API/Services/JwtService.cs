using HemaBazaar.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HemaBazaar.API.Services
{
    public class JwtService : IJwtService
    {
        JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string CreateToken(string userId, string email)
        {
            var claims = new List<Claim>
            {
                new Claim("userId",userId),
                new Claim("email",email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:_jwtSettings.Issuer, 
                audience:_jwtSettings.Audience, 
                claims:claims,
                expires:DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                signingCredentials: cred
            );


             return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
