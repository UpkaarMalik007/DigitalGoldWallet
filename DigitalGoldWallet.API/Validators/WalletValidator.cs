using DigitalGoldWallet.API.DTO;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class AddMoneyValidator : AbstractValidator<WalletAmountDTO>
    {
        public AddMoneyValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0");
        }
    }

    public class DeductMoneyValidator
        : AbstractValidator<WalletAmountDTO>
    {
        public DeductMoneyValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Deduction amount must be greater than 0");
        }
    }
}