using System.Text;
using Api.Data;
using Api.Helpers;
using Api.Interfaces;
using Api.Middleware;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
var cloud = builder.Configuration.GetSection("CloudinarySettings");
string corsapp = "corsapp";

builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseNpgsql(connection);
});
builder.Services.AddControllers();
builder.Services.AddCors(p => p.AddPolicy(corsapp, option =>
{
        option.WithOrigins("https://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<ITokenCreator, TokenCreator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<CloudSettings>(cloud);
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsapp);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
 var services = scope.ServiceProvider;
 try 
 {
     var context = services.GetRequiredService<DataContext>();
     await context.Database.MigrateAsync();
     await Seed.SeedUsers(context);
 }
 catch (Exception ex)
 {
     var logger = services.GetService<ILogger<Program>>();
     logger.LogError(ex, "An error occurred during migration");
 }

app.Run();
