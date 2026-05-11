using System.Security.Claims;

namespace DigitalGoldWallet.Tests.Helpers;

public static class TransactionClaimsPrincipalFactory
{
    public static ClaimsPrincipal CreateUser(int userId, string role = "User")
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        ];
        ClaimsIdentity identity = new(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}