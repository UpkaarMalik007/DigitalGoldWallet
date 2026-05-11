using DigitalGoldWallet.API.Dtos.AuthDto;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Invalid Id");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")

                // Example valid:
                // User@123
                // Admin@123
                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!]).{6,}$")
                .WithMessage("Invalid password format");
        }
    }
}