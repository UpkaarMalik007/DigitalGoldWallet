using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.ViewModels.Vendor;

public class VendorBranchViewModel
{
    public int BranchId { get; set; }

    public int? VendorId { get; set; }

    [Required(ErrorMessage = "Address ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Address ID must be greater than zero.")]
    public int? AddressId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Branch quantity cannot be negative.")]
    public decimal? Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string FullAddress
    {
        get
        {
            string[] parts =
            [
                Street ?? string.Empty,
                City ?? string.Empty,
                State ?? string.Empty,
                PostalCode ?? string.Empty,
                Country ?? string.Empty
            ];

            string address = string.Join(", ",
                parts.Where(part => !string.IsNullOrWhiteSpace(part)));

            return string.IsNullOrWhiteSpace(address)
                ? "Address not available"
                : address;
        }
    }
}
