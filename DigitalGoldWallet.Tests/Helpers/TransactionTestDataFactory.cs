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
        TransactionHistoryDto(),
        new TransactionHistoryDto
        {
            TransactionId = 2,
            UserId = 1,
            BranchId = 1,
            TransactionType = "Sell",
            TransactionStatus = "Success",
            Quantity = 5,
            Amount = 32000,
            CreatedAt = DateTime.UtcNow
        }
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

    public static FilterTransactionsDto FilterTransactionsDto() => new()
    {
        TransactionType = "Buy",
        TransactionStatus = "Success",
        FromDate = DateTime.UtcNow.AddMonths(-1),
        ToDate = DateTime.UtcNow
    };
}