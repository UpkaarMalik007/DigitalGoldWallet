using BCrypt.Net;


namespace DigitalGoldWallet.API.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
            }
            catch
            {
                // Fallback for plain text comparison if the stored password is not a valid hash
                return plainPassword == hashedPassword;
            }
        }
    }
}