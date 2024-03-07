using Jobs.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT authentication from appsettings.json
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        IConfigurationSection jwtSection = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = bool.Parse(jwtSection["ValidateIssuer"]),
            ValidateAudience = bool.Parse(jwtSection["ValidateAudience"]),
            ValidateLifetime = bool.Parse(jwtSection["ValidateLifetime"]),
            ValidateIssuerSigningKey = bool.Parse(jwtSection["ValidateIssuerSigningKey"]),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"])),
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"]
        };
    });

//DBCnnectin
builder.Services.AddDbContext<JobsDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("JobsConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
