using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DigitalGoldWallet.API.Configuration;
using DigitalGoldWallet.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DigitalGoldWallet.API.Helpers;

public class JwtHelper
{
    private readonly JwtSettings _jwtSettings;

    public JwtHelper(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateVendorToken(Vendor vendor)
    {
        List<Claim> claims =
        [
            new Claim("vendorId", vendor.VendorId.ToString()),
            new Claim(ClaimTypes.Role, "Vendor"),
            new Claim(ClaimTypes.Email, vendor.ContactEmail ?? string.Empty),
            new Claim(ClaimTypes.Name, vendor.VendorName)
        ];

        return GenerateToken(claims);
    }

    private string GenerateToken(List<Claim> claims)
    {
        SymmetricSecurityKey securityKey = new(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        SigningCredentials credentials = new(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}