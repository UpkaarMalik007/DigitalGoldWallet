namespace DigitalGoldWallet.API.DTO
{
    public class WalletAmountDTO
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }  = string.Empty;
    }

    // public class TransferMoneyDTO
    // {
    //     public int SenderId{get; set;}
    //     public int ReceiverId{get; set;}
    //     public decimal Amount{get; set;}
    // }
}