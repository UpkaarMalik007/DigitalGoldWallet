using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select login type")]
        public string LoginType { get; set; } = "User"; 
    }
}
