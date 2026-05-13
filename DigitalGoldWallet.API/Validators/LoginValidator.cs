using DigitalGoldWallet.API.Dtos.AuthDto;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}