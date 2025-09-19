using AuthMicroService.DTOs;
using AuthMicroService.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ILogger<AuthController> logger, IAuthService authService) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IAuthService _authService = authService;

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            await _authService.LoginAsync(HttpContext,loginDto);
            return Ok("Login Success");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync(HttpContext);
            return Ok("Logged out");
        }
    }
}
