using System.Security.Claims;
using AuthMicroService.DTOs;
using AuthMicroService.Entities;

namespace AuthMicroService.Services.IServices
{
    public interface IJwtService
    {
        string GenerateAccessToken(GenerateAccessTokenDto? generateAccessTokenDto);
        RefreshToken GenerateRefreshToken(string ipAddress);
        //ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
