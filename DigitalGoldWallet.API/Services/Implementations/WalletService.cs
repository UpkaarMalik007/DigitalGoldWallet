using System.Transactions;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interface;
using DigitalGoldWallet.API.Exceptions;

namespace DigitalGoldWallet.API.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<decimal> GetWalletBalance(int userId)
        {
            var user = await _walletRepository.GetUserById(userId);
            return user?.Balance ?? 0;
        }

        public async Task<string> AddMoney(AddMoneyDTO dto)
        {
            var user = await _walletRepository.GetUserById(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            user.Balance += dto.Amount;
            await _walletRepository.UpdateUser(user);
            await _walletRepository.SaveChanges();
            return "Money Added Successfully";
        }

        public async Task<string> DeductMoney(DeductMoneyDTO dto)
        {
            var user = await _walletRepository.GetUserById(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            if (user.Balance < dto.Amount)
            {
                throw new BadRequestException("Insufficient Balance");
            }
            
            user.Balance -= dto.Amount;
            await _walletRepository.UpdateUser(user);
            await _walletRepository.SaveChanges();
            return "Money Deducted Successfully";
        }

        public async Task<List<object>> GetWalletHistory(int userId)
        {
            var history = await _walletRepository.GetWalletHistory(userId);
            return history.Cast<object>().ToList();
        }

        public async Task<string> TransferMoney(TransferMoneyDTO dto)
        {
            var sender = await _walletRepository.GetUserById(dto.SenderId);
            var receiver = await _walletRepository.GetUserById(dto.ReceiverId);
            if(sender == null || receiver == null)
            {
                throw new NotFoundException("User Not Found");
            }
            if(sender.Balance < dto.Amount)
            {
                throw new BadRequestException("Insufficient Balance");
            }
            
            sender.Balance -= dto.Amount;
            receiver.Balance += dto.Amount;

            await _walletRepository.UpdateUser(sender);
            await _walletRepository.UpdateUser(receiver);

            await _walletRepository.SaveChanges();

            return "Transfer Money Successfully";
        }

        public async Task<object> GetLastTransaction(int userId)
        {
            var transaction = await _walletRepository
                .GetLastTransaction(userId);
            if (transaction == null)
            {
                return "No transaction found";
            }
            return transaction;
        }

        public async Task<object> GetWalletSummary(int userId)
        {
            var transactions = await _walletRepository.GetAllTransactions(userId);

            var totalCredit = transactions
                .Where(x => x.TransactionType == "Credit")
                .Sum(x => x.Amount);

            var totalDebit = transactions
                .Where(x => x.TransactionType == "Debit")
                .Sum(x => x.Amount);

            var user = await _walletRepository
                .GetUserById(userId);

            return new
            {
                Balance = user?.Balance,
                TotalCredit = totalCredit,
                TotalDebit = totalDebit,
                TotalTransaction = transactions.Count
            };
        }

        public async Task<int> GetTransactionsCount(int userId)
        {
            return await _walletRepository
                .GetTransactionsCount(userId);
        }

        public async Task<List<object>> GetTransactionsByDate(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _walletRepository
                .GetTransactionsByDate(userId, startDate, endDate);
            return transactions.Cast<object>().ToList();
        }

        public async Task<List<object>> GetTransactionsByStatus(int userId, string status)
        {
            var transactions = await _walletRepository
                .GetTransactionsByStatus(userId, status);
            return transactions.Cast<object>().ToList();
        }
    }
}