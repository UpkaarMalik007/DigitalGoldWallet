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

    public void ValidateGoldPrice(decimal currentGoldPrice)
    {
        if (currentGoldPrice <= 0)
        {
            throw new BadRequestException("Current gold price must be greater than zero.");
        }
    }

    public void ValidateQuantity(decimal quantity)
    {
        if (quantity < 0)
        {
            throw new BadRequestException("Branch quantity cannot be negative.");
        }
    }
}

public class CreateVendorDtoValidator : AbstractValidator<VendorDto>
{
    public CreateVendorDtoValidator()
    {
        RuleFor(dto => dto.VendorName)
            .NotEmpty()
            .WithMessage("Vendor name is required.")
            .Matches(@"^[A-Za-z0-9\s&.,'-]{2,100}$")
            .WithMessage("Vendor name must be 2 to 100 characters and can contain letters, numbers, spaces, &, comma, dot, apostrophe, and hyphen only.");

        RuleFor(dto => dto.ContactEmail)
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .When(dto => !string.IsNullOrWhiteSpace(dto.ContactEmail))
            .WithMessage("Invalid email format.");

        RuleFor(dto => dto.ContactPhone)
            .Matches(@"^\+?[0-9\s\-]{10,15}$")
            .When(dto => !string.IsNullOrWhiteSpace(dto.ContactPhone))
            .WithMessage("Invalid phone number format.");

        RuleFor(dto => dto.WebsiteUrl)
            .Matches(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$")
            .When(dto => !string.IsNullOrWhiteSpace(dto.WebsiteUrl))
            .WithMessage("Invalid website URL format.");

        RuleFor(dto => dto.CurrentGoldPrice)
            .NotNull()
            .WithMessage("Current gold price is required.")
            .GreaterThan(0)
            .WithMessage("Current gold price must be greater than zero.");

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$")
            .WithMessage("Password must be at least 6 characters and contain at least one letter and one number.");
    }
}

public class UpdateVendorDtoValidator : AbstractValidator<VendorDto>
{
    public UpdateVendorDtoValidator()
    {
        RuleFor(dto => dto.VendorName)
            .NotEmpty()
            .WithMessage("Vendor name is required.")
            .Matches(@"^[A-Za-z0-9\s&.,'-]{2,100}$")
            .WithMessage("Vendor name must be 2 to 100 characters and can contain letters, numbers, spaces, &, comma, dot, apostrophe, and hyphen only.");

        Include(new VendorContactFieldsValidator());
    }
}

public class UpdateVendorContactDtoValidator : AbstractValidator<VendorDto>
{
    public UpdateVendorContactDtoValidator()
    {
        RuleFor(dto => dto)
            .Must(dto =>
                !string.IsNullOrWhiteSpace(dto.ContactPersonName)
                || !string.IsNullOrWhiteSpace(dto.ContactEmail)
                || !string.IsNullOrWhiteSpace(dto.ContactPhone)
                || !string.IsNullOrWhiteSpace(dto.WebsiteUrl))
            .WithMessage("At least one contact field is required.");

        Include(new VendorContactFieldsValidator());
    }
}

public class VendorBranchDtoValidator : AbstractValidator<VendorBranchDto>
{
    public VendorBranchDtoValidator()
    {
        RuleFor(dto => dto.AddressId)
            .NotNull()
            .WithMessage("Address ID is required.")
            .GreaterThan(0)
            .WithMessage("Address ID must be greater than zero.");

        RuleFor(dto => dto.Quantity)
            .NotNull()
            .WithMessage("Branch quantity is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Branch quantity cannot be negative.");
    }
}

public class VendorContactFieldsValidator : AbstractValidator<VendorDto>
{
    public VendorContactFieldsValidator()
    {
        RuleFor(dto => dto.ContactEmail)
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .When(dto => !string.IsNullOrWhiteSpace(dto.ContactEmail))
            .WithMessage("Invalid email format.");

        RuleFor(dto => dto.ContactPhone)
            .Matches(@"^\+?[0-9\s\-]{10,15}$")
            .When(dto => !string.IsNullOrWhiteSpace(dto.ContactPhone))
            .WithMessage("Invalid phone number format.");

        RuleFor(dto => dto.WebsiteUrl)
            .Matches(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/[\w\-._~:/?#[\]@!$&'()*+,;=]*)?$")
            .When(dto => !string.IsNullOrWhiteSpace(dto.WebsiteUrl))
            .WithMessage("Invalid website URL format.");
    }
}