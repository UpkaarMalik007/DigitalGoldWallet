using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.ViewModels.Vendor;

public class AddressViewModel
{
    public int AddressId { get; set; }

    [Required(ErrorMessage = "Street is required.")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required.")]
    public string State { get; set; } = string.Empty;

    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    public string Country { get; set; } = string.Empty;

    public string FullAddress
    {
        get
        {
            string[] parts =
            [
                Street,
                City,
                State,
                PostalCode ?? string.Empty,
                Country
            ];

            string address = string.Join(", ",
                parts.Where(part => !string.IsNullOrWhiteSpace(part)));

            return string.IsNullOrWhiteSpace(address)
                ? "Address not available"
                : address;
        }
    }
}
