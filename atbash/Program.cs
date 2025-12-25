using Microsoft.EntityFrameworkCore;
using Atbash.Api.Data;
using Atbash.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<Atbash.Api.Services.AuthService>();

// DB 
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
// лайт
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connStr));

// DI
builder.Services.AddScoped<IAtbashService, AtbashService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();

var app = builder.Build();

// мигрируем
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// статика
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();