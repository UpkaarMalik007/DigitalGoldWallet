using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.Tests.Helpers;

public static class TransactionTestDataFactory
{
    public static TransactionHistoryDto TransactionHistoryDto() => new()
    {
        TransactionId = 1,
        UserId = 1,
        BranchId = 1,
        TransactionType = "Buy",
        TransactionStatus = "Success",
        Quantity = 10,
        Amount = 64000,
        CreatedAt = DateTime.UtcNow
    };

    public static List<TransactionHistoryDto> TransactionHistoryDtoList() => new()
    {
        new() { TransactionId = 1, UserId = 1, BranchId = 1,
                TransactionType = "Buy", TransactionStatus = "Success",
                Quantity = 10, Amount = 64000, CreatedAt = DateTime.UtcNow },
        new() { TransactionId = 2, UserId = 1, BranchId = 1,
                TransactionType = "Sell", TransactionStatus = "Success",
                Quantity = 5, Amount = 32000, CreatedAt = DateTime.UtcNow }
    };

    public static CreateTransactionDto CreateTransactionDto() => new()
    {
        UserId = 1,
        BranchId = 1,
        TransactionType = "Buy",
        TransactionStatus = "Success",
        Quantity = 2,
        Amount = 5000
    };

    public static CreateGoldOrderRequestDto GoldOrderRequestDto() => new()
    {
        BranchId = 1,
        Quantity = 2
    };

    public static CreateGoldOrderRequestDto InvalidGoldOrderRequestDto() => new()
    {
        BranchId = 0,
        Quantity = 2  // invalid branch
    };
}