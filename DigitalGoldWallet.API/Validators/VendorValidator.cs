using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators;

public class VendorValidator
{
    public void ValidateVendorId(int vendorId)
    {
        if (vendorId <= 0)
        {
            throw new BadRequestException("Vendor ID must be greater than zero.");
        }
    }

    public void ValidateBranchId(int branchId)
    {
        if (branchId <= 0)
        {
            throw new BadRequestException("Branch ID must be greater than zero.");
        }
    }

    public void ValidateSearchName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BadRequestException("Search name is required.");
        }
    }
}

public class CreateVendorDtoValidator : AbstractValidator<CreateVendorDto>
{
    public CreateVendorDtoValidator()
    {
        RuleFor(x => x.VendorName)
            .NotEmpty()
            .WithMessage("Vendor name is required.")
            .Matches(@"^[A-Za-z0-9\s&.,'-]{2,100}$")
            .WithMessage("Vendor name must be 2 to 100 characters and can contain letters, numbers, spaces, &, comma, dot, apostrophe, and hyphen only.");

        RuleFor(x => x.ContactEmail)
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("Invalid email format.");

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[0-9\s\-]{10,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone))
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.WebsiteUrl)
            .Matches(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$")
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl))
            .WithMessage("Invalid website URL format.");

        RuleFor(x => x.CurrentGoldPrice)
            .GreaterThan(0)
            .WithMessage("Current gold price must be greater than zero.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$")
            .WithMessage("Password must be at least 6 characters and contain at least one letter and one number.");
    }
}

public class UpdateVendorDtoValidator : AbstractValidator<UpdateVendorDto>
{
    public UpdateVendorDtoValidator()
    {
        RuleFor(x => x.VendorName)
            .NotEmpty()
            .WithMessage("Vendor name is required.")
            .Matches(@"^[A-Za-z0-9\s&.,'-]{2,100}$")
            .WithMessage("Vendor name must be 2 to 100 characters and can contain letters, numbers, spaces, &, comma, dot, apostrophe, and hyphen only.");

        RuleFor(x => x.ContactEmail)
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("Invalid email format.");

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[0-9\s\-]{10,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone))
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.WebsiteUrl)
            .Matches(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$")
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl))
            .WithMessage("Invalid website URL format.");
    }
}

public class UpdateVendorContactDtoValidator : AbstractValidator<UpdateVendorContactDto>
{
    public UpdateVendorContactDtoValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.ContactPersonName)
                || !string.IsNullOrWhiteSpace(x.ContactEmail)
                || !string.IsNullOrWhiteSpace(x.ContactPhone)
                || !string.IsNullOrWhiteSpace(x.WebsiteUrl))
            .WithMessage("At least one contact field is required.");

        RuleFor(x => x.ContactEmail)
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("Invalid email format.");

        RuleFor(x => x.ContactPhone)
            .Matches(@"^\+?[0-9\s\-]{10,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactPhone))
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.WebsiteUrl)
            .Matches(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$")
            .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl))
            .WithMessage("Invalid website URL format.");
    }
}

public class UpdateVendorPriceDtoValidator : AbstractValidator<UpdateVendorPriceDto>
{
    public UpdateVendorPriceDtoValidator()
    {
        RuleFor(x => x.CurrentGoldPrice)
            .GreaterThan(0)
            .WithMessage("Current gold price must be greater than zero.");
    }
}

public class CreateVendorBranchDtoValidator : AbstractValidator<CreateVendorBranchDto>
{
    public CreateVendorBranchDtoValidator()
    {
        RuleFor(x => x.AddressId)
            .GreaterThan(0)
            .WithMessage("Address ID must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Branch quantity cannot be negative.");
    }
}

public class UpdateBranchStockDtoValidator : AbstractValidator<UpdateBranchStockDto>
{
    public UpdateBranchStockDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Branch quantity cannot be negative.");
    }
}