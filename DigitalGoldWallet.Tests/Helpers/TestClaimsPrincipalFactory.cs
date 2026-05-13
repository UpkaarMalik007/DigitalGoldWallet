using System.Security.Claims;

namespace DigitalGoldWallet.Tests.Helpers;

public static class TransactionClaimsPrincipalFactory
{
    public static ClaimsPrincipal CreateUser(int userId = 1, string role = "User")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}