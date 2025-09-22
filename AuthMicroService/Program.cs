using System.Text;
using AuthMicroService;
using AuthMicroService.Configurations;
using AuthMicroService.Infrastructure;
using AuthMicroService.Mappings;
using AuthMicroService.Repositories;
using AuthMicroService.Repositories.IRepositories;
using AuthMicroService.Services;
using AuthMicroService.Services.IServices;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthServiceDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthServiceDb")));

//Logging 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

//Caching
builder.Services.AddRedisConfiguration(builder.Configuration);

//Mapper
builder.Services.AddAutoMapper(typeof(AuthMappingProfile));

//passwordHasher
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddHttpContextAccessor();
//Business Layer
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IJwtService, JwtService>();


//Repo Layer
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();
//Session based Authentication
// builder.Services.AddAuthentication("Cookies")
//     .AddCookie("Cookies", options =>
//     {
//         options.LoginPath = "/auth/login"; // optional
//         options.LogoutPath = "/auth/logout"; // optional
//         options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//         options.SlidingExpiration = true;
//     });

//JWT based Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; ;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    var config = builder.Configuration;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "")),
        ClockSkew = TimeSpan.Zero // optional: default is 5 min
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            if (ctx.Request.Cookies.ContainsKey("accessToken"))
            {
                ctx.Token = ctx.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(); // for [Authorize]

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173","http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // needed for cookies/session
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
