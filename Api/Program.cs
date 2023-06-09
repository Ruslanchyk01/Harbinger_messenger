using System.Text;
using Api.Data;
using Api.Interfaces;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
string corsapp = "corsapp";

builder.Services.AddScoped<ITokenCreator, TokenCreator>();
builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseNpgsql(connection);
});
builder.Services.AddControllers();
builder.Services.AddCors(p => p.AddPolicy(corsapp, option =>
{
        option.WithOrigins("https://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();
