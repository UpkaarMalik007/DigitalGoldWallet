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

        public async Task<object> GetWalletBalance(int userId)
        {
            var user = await _walletRepository.GetUserById(userId);
            if (user == null)
                throw new NotFoundException();
            return new
            {
                Balance = user.Balance,
                Name = user.Name
            };
        }

        public async Task<string> AddMoney(WalletAmountDTO dto)
        {
            var user = await _walletRepository.GetUserById(dto.UserId);
            if (user == null)
            {
                throw new NotFoundException();
            }
            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                throw new BadRequestException("Payment method is required");
            }
            user.Balance += dto.Amount;
            var payment = _mapper.Map<Payment>(dto);
            payment.PaymentMethod = dto.PaymentMethod;
            payment.TransactionType = "Credited to wallet";
            payment.PaymentStatus = "Success";
            payment.CreatedAt = DateTime.Now;
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
            return "Success";
        }

        // public async Task<List<object>> GetWalletHistory(int userId)
        // {
        //     var history = await _walletRepository.GetWalletHistory(userId);
        //     return history.Cast<object>().ToList();
        // }
        public async Task<List<object>> GetWalletHistory(int userId)
        {
            var history = await _walletRepository.GetWalletHistory(userId);
            var result = history
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.PaymentId,
                x.Amount,
                x.PaymentMethod,
                x.TransactionType,
                x.PaymentStatus,
                x.CreatedAt
            });
            return result.Cast<object>().ToList();
        }

        // public async Task<string> TransferMoney(TransferMoneyDTO dto)
        // {
        //     var sender = await _walletRepository.GetUserById(dto.SenderId);
        //     var receiver = await _walletRepository.GetUserById(dto.ReceiverId);
        //     if (sender == null || receiver == null)
        //     {
        //         throw new NotFoundException();
        //     }
        //     if (sender.Balance < dto.Amount)
        //     {
        //         throw new BadRequestException();
        //     }
        //     sender.Balance -= dto.Amount;
        //     receiver.Balance += dto.Amount;
        //     var payment = _mapper.Map<Payment>(dto);
        //     payment.UserId = sender.UserId;
        //     payment.PaymentMethod = "Bank Transfer";
        //     payment.TransactionType = "Debited from wallet";
        //     payment.PaymentStatus = "Success";
        //     payment.CreatedAt = DateTime.Now;
        //     await _walletRepository.UpdateUser(sender);
        //     await _walletRepository.UpdateUser(receiver);
        //     await _walletRepository.AddPayment(payment);
        //     await _walletRepository.SaveChanges();
        //     return "Transfer Money Successfully";
        // }

        public async Task<object> GetLastTransaction(int userId)
        {
            var transaction = await _walletRepository.GetLastTransaction(userId);
            
            if (transaction == null)
            {
                throw new NotFoundException();
            }
            // return transaction;
            return new
            {
                transaction.PaymentId,
                transaction.Amount,
                transaction.PaymentMethod,
                transaction.TransactionType,
                transaction.PaymentStatus,
                transaction.CreatedAt
            };
        }

        public async Task<object> GetWalletSummary(int userId)
        {
            var transactions =
                await _walletRepository
                    .GetAllTransactions(userId);
            var totalCredit = transactions
                .Where(x =>
                    x.TransactionType ==
                    "Credited to wallet")
                .Sum(x => x.Amount);
            var totalDebit = transactions
                .Where(x =>
                    x.TransactionType ==
                    "Debited from wallet")
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
            var transactions = await _walletRepository.GetTransactionsByDate(userId, startDate, endDate);
            // return transactions.Cast<object>().ToList();
            var result = transactions.Select(x => new
            {
                x.PaymentId,
                x.Amount,
                x.PaymentMethod,
                x.TransactionType,
                x.PaymentStatus,
                x.CreatedAt
            });
            return result.Cast<object>().ToList();
        }

        public async Task<List<object>> GetTransactionsByStatus(int userId, string status)
        {
            var transactions = await _walletRepository.GetTransactionsByStatus(userId, status);
            var result = transactions.Select(x => new
            {
                x.PaymentId,
                x.Amount,
                x.PaymentMethod,
                x.TransactionType,
                x.PaymentStatus,
                x.CreatedAt
            });
            return result.Cast<object>().ToList();
            // return transactions.Cast<object>().ToList();
        }

        public async Task<object> GetUsers()
        {
            var users =
                await _walletRepository.GetAllUsers();

            if (users == null || !users.Any())
            {
                throw new NotFoundException();
            }

            return users.Select(x => new
            {
                x.UserId,
                x.Name
            });
        }
    }
}