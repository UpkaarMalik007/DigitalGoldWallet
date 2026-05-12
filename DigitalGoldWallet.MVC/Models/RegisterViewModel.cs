using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public int? AddressId { get; set; }
    }
}
