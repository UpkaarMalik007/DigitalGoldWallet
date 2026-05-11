using DigitalGoldWallet.API.DTOs;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class TransactionValidator : AbstractValidator<CreateTransactionDto>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull().WithMessage("User ID is required.")
                .GreaterThan(0).WithMessage("User ID must be greater than 0.");

            RuleFor(x => x.BranchId)
                .NotNull().WithMessage("Branch ID is required.")
                .GreaterThan(0).WithMessage("Branch ID must be greater than 0.");


            RuleFor(x => x.TransactionType)
                .NotEmpty()
                .Must(type => type == "Buy" ||
                              type == "Sell" ||
                              type == "Convert to Physical")
                .WithMessage("Transaction type must be Buy, Sell, or Convert to Physical.");

            RuleFor(x => x.TransactionStatus)
                .NotEmpty()
                .Must(status => status == "Success" ||
                                status == "Failed")
                .WithMessage("Transaction status must be Success or Failed.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000 grams.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.")
                .LessThanOrEqualTo(10000000).WithMessage("Amount cannot exceed 1 crore.");
        }
    }
}
