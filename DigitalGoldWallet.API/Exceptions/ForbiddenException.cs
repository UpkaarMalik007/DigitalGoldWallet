namespace DigitalGoldWallet.API.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(
        string message = "Access Forbidden")
        : base(message)
    {
    }
}