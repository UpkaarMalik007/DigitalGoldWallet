namespace DigitalGoldWallet.API.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(
        string message = "Unauthorized Access")
        : base(message)
    {
    }
}