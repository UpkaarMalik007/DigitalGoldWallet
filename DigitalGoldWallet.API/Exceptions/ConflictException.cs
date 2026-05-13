namespace DigitalGoldWallet.API.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(
        string message = "Conflict Occurred")
        : base(message)
    {
    }
}