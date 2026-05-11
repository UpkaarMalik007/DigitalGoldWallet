using System.Security.Claims;

namespace DigitalGoldWallet.Tests.Helpers;

public static class TestClaimsPrincipalFactory
{
    public static ClaimsPrincipal CreateVendorUser(int vendorId)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, "Vendor"),
            new Claim("vendorId", vendorId.ToString())
        ];

        ClaimsIdentity identity = new(claims, "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipal CreateAdminUser()
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, "Admin")
        ];

        ClaimsIdentity identity = new(claims, "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipal CreateNormalUser()
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Role, "User")
        ];

        ClaimsIdentity identity = new(claims, "TestAuth");

        return new ClaimsPrincipal(identity);
    }
}