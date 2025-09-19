using AuthMicroService.DTOs;
using AuthMicroService.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMicroService.Services
{
    public class JwtService(IConfiguration configuration):IJwtService
    {
        private readonly IConfiguration _configuration=configuration;

        public AuthResponseDto GenerateAccessToken(GenerateAccessTokenDto? generateTokenDto)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,generateTokenDto?.UserId.ToString()??""),
                new(ClaimTypes.Name,generateTokenDto?.UserName??""),
                new(ClaimTypes.Email,generateTokenDto?.Email ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var role in generateTokenDto?.Roles ?? []) 
            {
                authClaims.Add(new Claim(ClaimTypes.Role,role));
            }

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]??"")
               );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])
                ),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var authResponse = new AuthResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return authResponse;
        }
    }
}
