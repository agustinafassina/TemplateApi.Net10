using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TemplateApi.Services.Implementations;
using TemplateApi.Services.Interfaces;
using TemplateApi.Configurations;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IItemService, ItemService>();

// CORS Configuration
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? (builder.Environment.IsDevelopment() ? new[] { "*" } : Array.Empty<string>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        if (builder.Environment.IsDevelopment() && allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// AutoMapper Configuration
builder.Services.AddMappers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Auth0App1", options =>
{
    options.Audience = configuration["Auth0App1:Audience"] ?? Environment.GetEnvironmentVariable("Auth0App1.Audience");
    options.Authority = configuration["Auth0App1:Issuer"] ?? Environment.GetEnvironmentVariable("Auth0App1.Issuer");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["Auth0App1:Issuer"] ?? Environment.GetEnvironmentVariable("Auth0App1.Issuer")
    };
})
.AddJwtBearer("Auth0App2", options =>
{
    options.Audience = configuration["Auth0App2:Audience"] ?? Environment.GetEnvironmentVariable("Auth0App2.Audience");
    options.Authority = configuration["Auth0App2:Issuer"] ?? Environment.GetEnvironmentVariable("Auth0App2.Issuer");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["Auth0App2:Issuer"] ?? Environment.GetEnvironmentVariable("Auth0App2.Issuer")
    };
});


builder.Services.AddControllers();

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Health Check endpoint
app.MapHealthChecks("/health");

app.Run();