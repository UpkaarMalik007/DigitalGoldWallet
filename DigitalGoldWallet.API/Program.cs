// Wallet - Himanshi 

using Microsoft.EntityFrameworkCore;
using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Repositories.Implementations;
using DigitalGoldWallet.API.Services.Interface;
using DigitalGoldWallet.API.Services.Implementations;
using DigitalGoldWallet.API.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using DigitalGoldWallet.API.Validators;


namespace DigitalGoldWallet.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddDbContext<DigitalGoldDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<AddMoneyValidator>();

            //Wallet - Himanshi
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<IWalletRepository, WalletRepository>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthorization();


            app.MapControllers();


            app.Run();

        }
    }
}

// Wallet - Himanshi
