using AuthMicroService.DTOs;

namespace AuthMicroService.Services.IServices
{
    public interface IJwtService
    {
        AuthResponseDto GenerateAccessToken(GenerateAccessTokenDto? generateAccessTokenDto);
    }
}
