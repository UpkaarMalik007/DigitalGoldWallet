using DigitalGoldWallet.API.DTOs.Gold;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators
{
    public class GoldActionRequestDtoValidator : AbstractValidator<GoldActionRequestDto>
    {
        public GoldActionRequestDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User Id is required.");

            RuleFor(x => x.ActionType)
                .IsInEnum().WithMessage("ActionType is required.");

            When(x => x.ActionType == GoldActionType.Buy, () =>
            {
                RuleFor(x => x.BranchId)
                    .NotNull().WithMessage("Branch Id is required for Buy.");
                RuleFor(x => x.Amount)
                    .NotNull().GreaterThan(0).WithMessage("Amount must be greater than zero for Buy.");
            });

            When(x => x.ActionType == GoldActionType.Sell, () =>
            {
                RuleFor(x => x.Quantity)
                    .NotNull().GreaterThan(0).WithMessage("Quantity must be greater than zero for Sell.");
            });

            When(x => x.ActionType == GoldActionType.Convert, () =>
            {
                RuleFor(x => x.BranchId)
                    .NotNull().WithMessage("Branch Id is required for Convert.");
                RuleFor(x => x.Quantity)
                    .NotNull().GreaterThan(0).WithMessage("Quantity must be greater than zero for Convert.");
                RuleFor(x => x.DeliveryAddressId)
                    .NotNull().WithMessage("Delivery Address Id is required for Convert.");
            });
        }
    }
}