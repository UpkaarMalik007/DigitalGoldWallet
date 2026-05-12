using DigitalGoldWallet.API.DTOs;
using FluentValidation;

namespace DigitalGoldWallet.API.Validators;

public class CreateUserDtoValidator
    : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s]+$")
            .WithMessage("Name should contain only alphabets");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$")
            .WithMessage("Password must contain uppercase, lowercase, number and special character");

        RuleFor(x => x.AddressId)
            .GreaterThan(0)
            .When(x => x.AddressId.HasValue)
            .WithMessage("AddressId must be greater than 0");
    }
}

public class UserDtoValidator
    : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Name should contain only alphabets");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.Password)
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Password must contain uppercase, lowercase, number and special character");
    }
}

public class AddressDtoValidator
    : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Street))
            .WithMessage("Street cannot exceed 255 characters");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.State)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.State))
            .WithMessage("State cannot exceed 100 characters");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode))
            .WithMessage("Postal code cannot exceed 20 characters");

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Country))
            .WithMessage("Country cannot exceed 100 characters");
    }
}