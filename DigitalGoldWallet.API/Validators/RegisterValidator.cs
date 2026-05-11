using DigitalGoldWallet.API.Dtos.AuthDto;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")

                // Aman Kumar
                .Matches(@"^[A-Za-z\s]+$")
                .WithMessage("Name should contain only alphabets");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")

                // aman.kumar@example.in
                .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")

                // User@123
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!]).{6,}$")
                .WithMessage("Password must contain uppercase, lowercase, number and special character");

            RuleFor(x => x.AddressId)
                .GreaterThan(0)
                .When(x => x.AddressId.HasValue)
                .WithMessage("Invalid AddressId");
        }
    }
}