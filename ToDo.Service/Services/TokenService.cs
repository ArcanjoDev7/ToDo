using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo.Domain.Models;
using ToDo.Service.Services.Interfaces;

namespace ToDo.Service.Services
{
    public class TokenService(string secret) : ITokenService
    {
        protected readonly string SecretKey = secret;
        public string GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(SecretKey);

            var claims = new Claim[]
            {

                new Claim(ClaimTypes.Name, user.Email),
                                                        
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
