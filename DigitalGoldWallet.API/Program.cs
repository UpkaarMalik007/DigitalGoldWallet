using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DigitalGoldWallet.API.Data;
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

namespace DigitalGoldWallet.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddDbContext<DigitalGoldDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

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

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

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

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();

            app.UseAuthentication();

            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthorization();

            

            app.MapControllers();

            app.Run();

        }
    }
}





