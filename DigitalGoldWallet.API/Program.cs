using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
// Wallet - Himanshi 

using Microsoft.EntityFrameworkCore;
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
using DigitalGoldWallet.API.Helpers;
using DigitalGoldWallet.API.Middleware;
using DigitalGoldWallet.API.Middlewares;
using DigitalGoldWallet.API.Repositories;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Repositories.Implementations;
using DigitalGoldWallet.API.Services.Interface;
using DigitalGoldWallet.API.Services.Implementations;
using DigitalGoldWallet.API.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using DigitalGoldWallet.API.Validators;


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
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection"));
            Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddDbContext<DigitalGoldDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<AddMoneyValidator>();

            //Wallet - Himanshi
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Vendor FluentValidation validators
builder.Services.AddScoped<IValidator<CreateVendorDto>, CreateVendorDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVendorDto>, UpdateVendorDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVendorContactDto>, UpdateVendorContactDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateVendorPriceDto>, UpdateVendorPriceDtoValidator>();
builder.Services.AddScoped<IValidator<CreateVendorBranchDto>, CreateVendorBranchDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateBranchStockDto>, UpdateBranchStockDtoValidator>();
            builder.Services.AddFluentValidationAutoValidation();
            

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DigitalGoldWallet API",
                    Version = "v1"
                });

                // JWT Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Token like this: Bearer your_token"
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

            //Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("User@123"));
            //Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Vendor@123"));
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "DigitalGoldWallet API",
                        Version = "v1"
                    });

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",

                        Type = SecuritySchemeType.Http,

                        Scheme = "bearer",

                        BearerFormat = "JWT",

                        In = ParameterLocation.Header,

                        Description =
                            "Enter JWT Token"
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
                    });
            });

            // =========================================
            // DEPENDENCY INJECTION
            // =========================================

            builder.Services.AddScoped<IUserRepository,
                UserRepository>();

            builder.Services.AddScoped<IUserService,
                UserService>();

            builder.Services.AddScoped<JwtHelper>();
            builder.Services.AddAutoMapper(typeof(Program));

            // =========================================
            // JWT AUTHENTICATION
            // =========================================

            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        builder.Configuration["Jwt:Key"]!))
                        };

                    options.Events = new JwtBearerEvents
                    {
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                Message = "You do not have permission to access this resource"
                            });
                        },

                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

WebApplication app = builder.Build();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
                            await context.Response.WriteAsJsonAsync(new
                            {
                                Message = "You are not authorized. Please login first"
                            });
                        }
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();

            app.UseAuthentication();

            app.UseMiddleware<JwtMiddleware>();
            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthorization();

            app.UseAuthorization();

app.MapControllers();
            

            app.MapControllers();

app.Run();
            app.Run();

public partial class Program { }

// Done By: Ekta


// Wallet - Himanshi
