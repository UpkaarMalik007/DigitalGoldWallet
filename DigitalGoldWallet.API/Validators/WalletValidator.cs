using DigitalGoldWallet.API.DTO;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    // Add Money Validation
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

    // Deduct Money Validation
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

    // // Transfer Money Validation
    // public class TransferMoneyValidator
    //     : AbstractValidator<TransferMoneyDTO>
    // {
    //     public TransferMoneyValidator()
    //     {
    //         RuleFor(x => x.SenderId)
    //             .GreaterThan(0);

    //         RuleFor(x => x.ReceiverId)
    //             .GreaterThan(0);

    //         RuleFor(x => x.Amount)
    //             .GreaterThan(0);

    //         RuleFor(x => x)
    //             .Must(x => x.SenderId != x.ReceiverId)
    //             .WithMessage("Sender and Receiver cannot be same");
    //     }
    // }
}