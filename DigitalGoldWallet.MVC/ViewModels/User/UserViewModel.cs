using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.ViewModels.User;

public class UserViewModel
{
    public int UserId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    public int? AddressId { get; set; }

    public decimal Balance { get; set; }

    public AddressViewModel? Address { get; set; }
}