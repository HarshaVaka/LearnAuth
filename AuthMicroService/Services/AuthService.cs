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
    public class AuthService(IAuthRepo authRepo, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, IConfiguration configuration, IRefreshTokenRepo refreshTokenRepo, IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IAuthRepo _authRepo = authRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IRefreshTokenRepo _refreshTokenRepo = refreshTokenRepo;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, string ipAddress)
        {
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = _passwordHasher.HashPassword(registerDto.Password);
            user.CreatedDate = DateTime.UtcNow;
            user.UserId = Guid.NewGuid();

            user = await _authRepo.RegisterAsync(user);
            var generateTokenDto = _mapper.Map<GenerateAccessTokenDto>(user);
            var accessToken = _jwtService.GenerateAccessToken(generateTokenDto);
            var refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.UserId;

            await _refreshTokenRepo.AddRefreshTokenAsync(refreshToken);
            var response = _httpContextAccessor.HttpContext!.Response;
            response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshToken.ExpiresAt
            });
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])
                )
            };
        }

        public async Task<AuthResponseDto> LoginAsync(HttpContext httpContext, LoginDto loginDto, string ipAddress)
        {
            User? user;
            user = await _authRepo.GetUserByEmailAsync(loginDto.Email ?? "");
            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user.PasswordHash, loginDto.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    var generateTokenDto = _mapper.Map<GenerateAccessTokenDto>(user);
                    var accessToken = _jwtService.GenerateAccessToken(generateTokenDto);
                    RefreshToken refreshToken = _jwtService.GenerateRefreshToken(ipAddress);
                    refreshToken.UserId = user.UserId;

                    await _refreshTokenRepo.AddRefreshTokenAsync(refreshToken);
                    var response = _httpContextAccessor.HttpContext!.Response;
                    response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = refreshToken.ExpiresAt
                    });

                    return new AuthResponseDto
                    {
                        AccessToken = accessToken,
                        AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                            Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]))
                    };
                    //Session or Cookie based auth 
                    // var claims = new List<Claim>
                    // {
                    //     new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    //     new(ClaimTypes.Name, user.UserName??""), 
                    // };
                    // foreach (var userRole in user.UserRoles)
                    // {
                    //     claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                    // }

                    // var claimsIdentity = new ClaimsIdentity(
                    //     claims,
                    //     CookieAuthenticationDefaults.AuthenticationScheme
                    // );
                    // var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    // await httpContext.SignInAsync(
                    //     CookieAuthenticationDefaults.AuthenticationScheme,
                    //     claimsPrincipal,
                    //     new AuthenticationProperties
                    //     {
                    //         IsPersistent = true,
                    //         ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    //     }
                    //     );
                }
                else
                {
                    throw new ApiException("Incorrect Password", StatusCodes.Status401Unauthorized);
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
        
        public async Task<AuthResponseDto> RefreshTokenAsync(HttpContext httpContext, string ipAddress)
        {
            var request = httpContext.Request;
            var refreshToken = request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ApiException("Refresh Token is missing", StatusCodes.Status400BadRequest);
            }
            var existingRefreshToken = await _refreshTokenRepo.GetRefreshTokenAsync(refreshToken);
            if (existingRefreshToken == null || !existingRefreshToken.IsActive)
            {
                throw new ApiException("Invalid Refresh Token", StatusCodes.Status400BadRequest);
            }
            var user = await _authRepo.GetUserByUserIdAsync(existingRefreshToken.UserId);
            if (user == null)
            {
                throw new ApiException("User Not Found", StatusCodes.Status404NotFound);
            }
            //generate new refresh token and access token
            var newAccessToken = _jwtService.GenerateAccessToken(_mapper.Map<GenerateAccessTokenDto>(user));
            var newRefreshToken = _jwtService.GenerateRefreshToken(ipAddress);
            newRefreshToken.UserId = user.UserId;

            //revoke old refresh token
            existingRefreshToken.Revoked = DateTime.UtcNow;
            existingRefreshToken.RevokedByIp = ipAddress;
            existingRefreshToken.ReplacedByToken = newRefreshToken.Token;

            await _refreshTokenRepo.UpdateRefreshTokenAsync(existingRefreshToken);
            await _refreshTokenRepo.AddRefreshTokenAsync(newRefreshToken);

            var response = httpContext.Response;
            response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = newRefreshToken.ExpiresAt
            });

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])
                )
            };
        }
    }
}
