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
using Microsoft.EntityFrameworkCore;
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

//Business Layer
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IJwtService, JwtService>();

//Repo Layer
builder.Services.AddScoped<IAuthRepo, AuthRepo>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/auth/login"; // optional
        options.LogoutPath = "/auth/logout"; // optional
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(); // for [Authorize]

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
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

app.UseAuthorization();

app.MapControllers();

app.Run();
