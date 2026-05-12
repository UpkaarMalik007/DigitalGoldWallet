namespace DigitalGoldWallet.API.Dtos.AuthDto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string? Password { get; set; }
    }
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? AddressId { get; set; }
    }

    public class AuthResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

}