using DigitalGoldWallet.API.DTOs.Gold;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class BuyGoldDtoValidator : AbstractValidator<BuyGoldDto>
    {
        public BuyGoldDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User Id is required.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch Id is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }

    public class SellGoldDtoValidator : AbstractValidator<SellGoldDto>
    {
        public SellGoldDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User Id is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }

    public class ConvertToPhysicalDtoValidator : AbstractValidator<ConvertToPhysicalDto>
    {
        public ConvertToPhysicalDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User Id is required.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Branch Id is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.DeliveryAddressId)
                .GreaterThan(0).WithMessage("Delivery Address Id is required.");
        }
    }
}