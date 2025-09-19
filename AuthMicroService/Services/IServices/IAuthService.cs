using AuthMicroService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroService.Services.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        Task LoginAsync(HttpContext httpContext,LoginDto loginDto);

        Task SignOutAsync(HttpContext httpContext); 
    }
}
