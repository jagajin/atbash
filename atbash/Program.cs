using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Atbash.Api.Data;
using Atbash.Api.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// 1. Регистрация сервисов
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500") // Порт вашего фронтенда
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // КРИТИЧЕСКИ ВАЖНО для аутентификации
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Настройка БД
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connStr));

// 3. Аутентификация (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });

// ✅ 4. КРИТИЧЕСКИ ВАЖНАЯ СТРОКА - РЕГИСТРАЦИЯ АВТОРИЗАЦИИ
builder.Services.AddAuthorization();

// 5. Регистрация сервисов
builder.Services.AddScoped<IAtbashService, AtbashService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<ITextService, TextService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();
app.UseCors("AllowFrontend"); 

// 6. Middleware в правильном порядке
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles();
app.UseRouting();

// ✅ ОБЯЗАТЕЛЬНЫЙ ПОРЯДОК:
app.UseAuthentication(); // Проверяет токен
app.UseAuthorization();  // Проверяет права

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();