namespace DigitalGoldWallet.API.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(
        string message = "Resource Not Found")
        : base(message)
    {
    }
}