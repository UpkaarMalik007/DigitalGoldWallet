using System.Text;
using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Helpers;
using DigitalGoldWallet.API.Middleware;
using DigitalGoldWallet.API.Repositories.Implementations;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Implementations;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.API.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Done By: Ekta

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only. Do not type Bearer manually."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<DigitalGoldDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<DigitalGoldWallet.API.Configuration.JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

DigitalGoldWallet.API.Configuration.JwtSettings jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<DigitalGoldWallet.API.Configuration.JwtSettings>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<JwtHelper>();

// Vendor module registrations
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorService, VendorService>();

// Custom route/query validator
builder.Services.AddScoped<VendorValidator>();

// Vendor FluentValidation validators
builder.Services.AddScoped<IValidator<CreateVendorDto>, CreateVendorDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVendorDto>, UpdateVendorDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVendorContactDto>, UpdateVendorContactDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVendorPriceDto>, UpdateVendorPriceDtoValidator>();
builder.Services.AddScoped<IValidator<CreateVendorBranchDto>, CreateVendorBranchDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateBranchStockDto>, UpdateBranchStockDtoValidator>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

// Done By: Ekta