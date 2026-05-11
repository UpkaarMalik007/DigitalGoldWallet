using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
using FluentValidation;
using Razorpay.Api;

namespace DigitalGoldWallet.API.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository; // services dependency injection on repo
        private readonly IVendorRepository _vendorRepository; // to validate branch and vendor details during order creation
        private readonly IConfiguration _config; //for razorpay keys from appsettings.json
        private readonly IValidator<CreateTransactionDto> _validator; // using validator here

        //Constructor injection for all dependencies
        public TransactionService(
            ITransactionRepository transactionRepository,
            IVendorRepository vendorRepository,
            IConfiguration config,
            IValidator<CreateTransactionDto> validator)
        {
            _transactionRepository = transactionRepository;
            _vendorRepository = vendorRepository;
            _config = config;
            _validator = validator;
        }

        // return all transaction history for a specific user
        public async Task<List<TransactionHistoryDto>> GetHistoryAsync(int userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<TransactionHistoryDto?> GetTransactionByIdAsync(int userId, int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null || transaction.UserId != userId)
                return null;

            return MapToDto(transaction);
        }

        public async Task<List<TransactionHistoryDto>> GetFilteredAsync(int userId, FilterTransactionsDto dto)
        {
            var transactions = await _transactionRepository.GetFilteredAsync(
                userId,
                dto.TransactionType,
                dto.TransactionStatus,
                dto.FromDate,
                dto.ToDate);

            return transactions.Select(MapToDto).ToList();
        }

        public async Task<List<TransactionHistoryDto>> GetRecentActivityAsync(int userId)
        {
            var transactions = await _transactionRepository.GetRecentActivityAsync(userId);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<string?> GetTransactionStatusAsync(int userId, int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null || transaction.UserId != userId)
                return null;

            return transaction.TransactionStatus;
        }

        public async Task<List<TransactionHistoryDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<List<TransactionHistoryDto>> GetMonthlyReportAsync(int month, int year)
        {
            var transactions = await _transactionRepository.GetMonthlyReportAsync(month, year);
            return transactions.Select(MapToDto).ToList();
        }
         
        public async Task<List<TransactionHistoryDto>> GetFinancialLogAsync()
        {
            var transactions = await _transactionRepository.GetFinancialLogAsync();
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<bool> UpdateTransactionStatusAsync(int transactionId, string transactionStatus)
        {
            if (string.IsNullOrWhiteSpace(transactionStatus))
                throw new BadRequestException("Transaction status cannot be empty");

            if (transactionStatus != "Success" && transactionStatus != "Failed")
                throw new BadRequestException("Status must be Success or Failed");

            return await _transactionRepository
                .UpdateTransactionStatusAsync(
                    transactionId,
                    transactionStatus);
        }

        // This method creates a Razorpay order for purchasing gold and returns the order details needed for the frontend to complete the payment process.
        // It also performs necessary validations on the input data and checks the availability of gold in the specified branch.
        public async Task<object> CreateOrderAsync(CreateGoldOrderRequestDto dto)
        {
            if (dto.BranchId <= 0)
                throw new BadRequestException("Invalid branch");

            if (dto.Quantity <= 0)
                throw new BadRequestException("Invalid quantity");

            var branch = await _vendorRepository.GetBranchByIdAsync(dto.BranchId);

            if (branch == null)
                throw new NotFoundException("Branch not found");

            if (branch.Vendor == null)
                throw new NotFoundException("Vendor not found");

            if (branch.Quantity < dto.Quantity)
                throw new BadRequestException("Not enough gold available");

            decimal amount = branch.Vendor.CurrentGoldPrice * dto.Quantity;

            var key = _config["Razorpay:Key"];
            var secret = _config["Razorpay:Secret"];

            RazorpayClient client = new RazorpayClient(key, secret);

            var options = new Dictionary<string, object>
            {
                { "amount", Convert.ToInt32(amount * 100) }, //Razorpay needs amount in paise (not rupees)
                { "currency", "INR" },
                { "receipt", Guid.NewGuid().ToString() } //Creates a unique receipt ID for every orde
            };

            Order order = client.Order.Create(options);

            return new
            {
                orderId = order["id"].ToString(),
                amount = Convert.ToInt32(order["amount"]),
                currency = order["currency"].ToString(),
                branchId = dto.BranchId,
                quantity = dto.Quantity
            };
        }

        // This method creates a new transaction record in the database based on the provided CreateTransactionDto.
        // It first validates the input data using FluentValidation, then constructs a TransactionHistory entity and saves it to the database through the repository. Finally, it returns a TransactionHistoryDto representing the newly created transaction.
        public async Task<TransactionHistoryDto> CreateTransactionAsync(CreateTransactionDto dto)
        {
            var result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var transaction = new TransactionHistory
            {
                UserId = dto.UserId,
                BranchId = dto.BranchId,
                TransactionType = dto.TransactionType,
                TransactionStatus = dto.TransactionStatus,
                Quantity = dto.Quantity,
                Amount = dto.Amount,
                CreatedAt = DateTime.Now
            };

            var savedTransaction = await _transactionRepository.AddAsync(transaction);

            return MapToDto(savedTransaction);
        }

        public async Task<List<VendorTransactionDto>> GetVendorTransactionsAsync(int vendorId)
        {
            var transactions = await _transactionRepository.GetVendorTransactionsAsync(vendorId);

            return transactions.Select(MapToVendorDto).ToList();
        }

        // This method calculates a summary of transactions for a specific vendor, including total transactions, successful transactions,
        // failed transactions, total buy amount, total sell amount, and total revenue.
        // It retrieves all transactions for the vendor and performs the necessary calculations
        //which is then returned to the caller.

        public async Task<VendorTransactionSummaryDto> GetVendorTransactionSummaryAsync(int vendorId)
        {
            var transactions = await _transactionRepository.GetVendorTransactionsAsync(vendorId);

            return new VendorTransactionSummaryDto
            {
                TotalTransactions = transactions.Count,

                SuccessfulTransactions = transactions.Count(t =>
                    t.TransactionStatus == "Success"),

                FailedTransactions = transactions.Count(t =>
                    t.TransactionStatus == "Failed"),

                TotalBuyAmount = transactions
                    .Where(t => t.TransactionType == "Buy" && t.TransactionStatus == "Success")
                    .Sum(t => t.Amount),

                TotalSellAmount = transactions
                    .Where(t => t.TransactionType == "Sell" && t.TransactionStatus == "Success")
                    .Sum(t => t.Amount),

                TotalRevenue = transactions
                    .Where(t => t.TransactionStatus == "Success")
                    .Sum(t => t.Amount)
            };
        }

        public async Task<List<VendorTransactionDto>> GetVendorSuccessfulTransactionsAsync(int vendorId)
        {
            var transactions = await _transactionRepository.GetVendorSuccessfulTransactionsAsync(vendorId);

            return transactions.Select(MapToVendorDto).ToList();
        }


        // converts each TransactionHistory entity to a TransactionHistoryDto for API responses 
        private static TransactionHistoryDto MapToDto(TransactionHistory transaction)
        {
            return new TransactionHistoryDto
            {
                TransactionId = transaction.TransactionId,
                UserId = transaction.UserId,
                BranchId = transaction.BranchId,
                TransactionType = transaction.TransactionType,
                TransactionStatus = transaction.TransactionStatus,
                Quantity = transaction.Quantity,
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt
            };
        }

        private static VendorTransactionDto MapToVendorDto(TransactionHistory t)
        {
            return new VendorTransactionDto
            {
                TransactionId = t.TransactionId,
                UserId = t.UserId,
                BranchId = t.BranchId,
                TransactionType = t.TransactionType,
                TransactionStatus = t.TransactionStatus,
                Quantity = t.Quantity,
                Amount = t.Amount,
                CreatedAt = t.CreatedAt
            };
        }


    }
}