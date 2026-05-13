using System.Transactions;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.API.Exceptions;
using AutoMapper;

namespace DigitalGoldWallet.API.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        // public WalletService(IWalletRepository walletRepository)
        // {
        //     _walletRepository = walletRepository;
        // }
        public WalletService(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<decimal> GetWalletBalance(int userId)
        {
            var user = await _walletRepository.GetUserById(userId);
            return user?.Balance ?? 0;
        }

        public async Task<string> AddMoney(WalletAmountDTO dto)
        {
            var user = await _walletRepository.GetUserById(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException();
            }
            user.Balance += dto.Amount;
            var payment = _mapper.Map<Payment>(dto);
            await _walletRepository.UpdateUser(user);
            await _walletRepository.AddPayment(payment);
            await _walletRepository.SaveChanges();
            return "Success";
        }

        public async Task<string> DeductMoney(WalletAmountDTO dto)
        {
            var user = await _walletRepository.GetUserById(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException();
            }
            if (user.Balance < dto.Amount)
            {
                throw new BadRequestException();
            }
            
            user.Balance -= dto.Amount;
            var payment = _mapper.Map<Payment>(dto);
            await _walletRepository.UpdateUser(user);
            await _walletRepository.AddPayment(payment);
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
                throw new NotFoundException();
            }
            if(sender.Balance < dto.Amount)
            {
                throw new BadRequestException();
            }
            
            sender.Balance -= dto.Amount;
            receiver.Balance += dto.Amount;

            var payment = _mapper.Map<Payment>(dto);
            await _walletRepository.UpdateUser(sender);
            await _walletRepository.UpdateUser(receiver);
            await _walletRepository.AddPayment(payment);
            await _walletRepository.SaveChanges();

            return "Transfer Money Successfully";
        }

        public async Task<object> GetLastTransaction(int userId)
        {
            var transaction = await _walletRepository
                .GetLastTransaction(userId);
            if (transaction == null)
            {
                throw new NotFoundException();
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