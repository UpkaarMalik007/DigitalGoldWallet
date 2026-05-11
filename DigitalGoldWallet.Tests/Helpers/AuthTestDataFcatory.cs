using DigitalGoldWallet.API.Dtos.AuthDto;

namespace DigitalGoldWallet.Tests.Helpers;

public static class AuthTestDataFactory
{
    public static LoginDto LoginDto() => new()
    {
        Email = "test@example.in",
        Password = "Test@123"
    };

    public static RegisterDto RegisterDto() => new()
    {
        Email = "newuser@example.in",
        Name = "New User",
        Password = "Test@123",
        AddressId = 1
    };

    public static AuthResponseDto AuthResponseDto() => new()
    {
        Message = "Login successful",
        Token = "fake-token",
        Id = 1,
        Name = "Test User",
        Role = "User"
    };
}