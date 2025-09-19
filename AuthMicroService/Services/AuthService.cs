using AuthMicroService.DTOs;
using AuthMicroService.Entities;
using AuthMicroService.Exceptions;
using AuthMicroService.Repositories.IRepositories;
using AuthMicroService.Services.IServices;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AuthMicroService.Services
{
    public class AuthService(IAuthRepo authRepo,IMapper mapper,IPasswordHasher passwordHasher,IJwtService jwtService):IAuthService
    {
        private readonly IAuthRepo _authRepo = authRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtService _jwtService = jwtService;

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
            user.CreatedDate = DateTime.UtcNow;
            user.UserId = Guid.NewGuid();

            user = await _authRepo.RegisterAsync(user);

            //Return token response

            var generateTokenDto = _mapper.Map<GenerateAccessTokenDto>(user);
            var token = _jwtService.GenerateAccessToken(generateTokenDto);
            return token;
        }

        public async Task LoginAsync(HttpContext httpContext, LoginDto loginDto)
        {
            User? user;
            user = await _authRepo.GetUserByEmailAsync(loginDto.Email ?? "");
            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user.PasswordHash, loginDto.Password);
                if (result == PasswordVerificationResult.Success)
                {

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new(ClaimTypes.Name, user.UserName??""), 
                    };
                    foreach (var userRole in user.UserRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                        }
                        );
                }
                else
                {
                    throw new ApiException("Incorrect Password",StatusCodes.Status401Unauthorized);
                }
            }
            else
            {
                throw new ApiException("User Not Found", StatusCodes.Status401Unauthorized);
            }
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
