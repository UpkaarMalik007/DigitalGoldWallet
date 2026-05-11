namespace DigitalGoldWallet.API.DTO
{
    // public class AddMoneyDTO
    // {
    //     public int UserId{get; set;}
    //     public decimal Amount{get; set;}
    // }

    // public class DeductMoneyDTO
    // {
    //     public int UserId{get; set;}
    //     public decimal Amount{get; set;}
    // }

    public class WalletAmountDTO
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }
    }

    public class TransferMoneyDTO
    {
        public int SenderId{get; set;}
        public int ReceiverId{get; set;}
        public decimal Amount{get; set;}
    }
}