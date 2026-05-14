using AutoMapper;
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
        private readonly ITransactionRepository _transactionRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IConfiguration _config;
        private readonly IValidator<CreateTransactionDto> _validator;
        private readonly IMapper _mapper;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IVendorRepository vendorRepository,
            IConfiguration config,
            IValidator<CreateTransactionDto> validator,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _vendorRepository = vendorRepository;
            _config = config;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<List<TransactionHistoryDto>> GetHistoryAsync(int userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);

            return _mapper.Map<List<TransactionHistoryDto>>(transactions);
        }

        public async Task<TransactionHistoryDto> GetTransactionByIdAsync(
            int userId,
            int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction == null)
                throw new NotFoundException("Transaction not found.");

            if (transaction.UserId != userId)
                throw new ForbiddenException("You do not have access to this transaction.");

            return _mapper.Map<TransactionHistoryDto>(transaction);
        }

        public async Task<List<TransactionHistoryDto>> GetFilteredAsync(
            int userId,
            FilterTransactionsDto dto)
        {
            var transactions = await _transactionRepository.GetFilteredAsync(
                userId,
                dto.TransactionType,
                dto.TransactionStatus,
                dto.FromDate,
                dto.ToDate);

            return _mapper.Map<List<TransactionHistoryDto>>(transactions);
        }

        public async Task<TransactionHistoryDto> CreateTransactionAsync(
            CreateTransactionDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var transaction = _mapper.Map<TransactionHistory>(dto);
            transaction.CreatedAt = DateTime.Now;

            var savedTransaction = await _transactionRepository.AddAsync(transaction);

            return _mapper.Map<TransactionHistoryDto>(savedTransaction);
        }

        //public async Task<object> CreateOrderAsync(int branchId, decimal quantity)
        //{
        //    if (branchId <= 0)
        //        throw new BadRequestException("Invalid branch.");

        //    if (quantity <= 0)
        //        throw new BadRequestException("Invalid quantity.");

        //    var branch = await _vendorRepository.GetBranchByIdAsync(branchId);

        //    if (branch == null)
        //        throw new NotFoundException("Branch not found.");

        //    if (branch.Vendor == null)
        //        throw new NotFoundException("Vendor not found.");

        //    if (branch.Quantity < quantity)
        //        throw new BadRequestException("Not enough gold available.");

        //    decimal amount = branch.Vendor.CurrentGoldPrice * quantity;

        //    var key = _config["Razorpay:Key"];
        //    var secret = _config["Razorpay:Secret"];

        //    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
        //        throw new BadRequestException("Razorpay configuration is missing.");

        //    RazorpayClient client = new RazorpayClient(key, secret);

        //    var options = new Dictionary<string, object>
        //    {
        //        { "amount", Convert.ToInt32(amount * 100) },
        //        { "currency", "INR" },
        //        { "receipt", Guid.NewGuid().ToString() }
        //    };

        //    Order order = client.Order.Create(options);

        //    return new
        //    {
        //        OrderId = order["id"].ToString(),
        //        Amount = Convert.ToInt32(order["amount"]),
        //        Currency = order["currency"].ToString(),
        //        BranchId = branchId,
        //        Quantity = quantity
        //    };
        //}

        public async Task<List<TransactionHistoryDto>> GetAllTransactionsAsync(
                int pageNumber, int pageSize)
        {
            ValidatePagination(pageNumber, pageSize);

            var transactions = await _transactionRepository.GetAllAsync(
                pageNumber,
                pageSize);

            return _mapper.Map<List<TransactionHistoryDto>>(transactions);
        }

        public async Task<List<TransactionHistoryDto>> GetMonthlyReportAsync(
            int month,
            int year)
        {
            if (month < 1 || month > 12)
                throw new BadRequestException("Month must be between 1 and 12.");

            if (year <= 0)
                throw new BadRequestException("Invalid year.");

            var transactions = await _transactionRepository.GetMonthlyReportAsync(
                month,
                year);

            return _mapper.Map<List<TransactionHistoryDto>>(transactions);
        }

        public async Task<bool> UpdateTransactionStatusAsync(
            int transactionId,
            string transactionStatus)
        {
            if (string.IsNullOrWhiteSpace(transactionStatus))
                throw new BadRequestException("Transaction status cannot be empty.");

            if (transactionStatus != "Success" && transactionStatus != "Failed")
                throw new BadRequestException("Status must be Success or Failed.");

            var updated = await _transactionRepository.UpdateTransactionStatusAsync(
                transactionId,
                transactionStatus);

            if (!updated)
                throw new NotFoundException("Transaction not found.");

            return true;
        }

        public async Task<List<TransactionHistoryDto>> GetVendorTransactionsAsync(
            int vendorId)
        {
            var transactions = await _transactionRepository
                .GetVendorTransactionsAsync(vendorId);

            return _mapper.Map<List<TransactionHistoryDto>>(transactions);
        }

        public async Task<VendorTransactionSummaryDto> GetVendorTransactionSummaryAsync(
            int vendorId)
        {
            var transactions = await _transactionRepository
                .GetVendorTransactionsAsync(vendorId);

            return new VendorTransactionSummaryDto
            {
                TotalTransactions = transactions.Count,

                SuccessfulTransactions = transactions.Count(t =>
                    t.TransactionStatus == "Success"),

                FailedTransactions = transactions.Count(t =>
                    t.TransactionStatus == "Failed"),

                TotalBuyAmount = transactions
                    .Where(t => t.TransactionType == "Buy" &&
                                t.TransactionStatus == "Success")
                    .Sum(t => t.Amount),

                TotalSellAmount = transactions
                    .Where(t => t.TransactionType == "Sell" &&
                                t.TransactionStatus == "Success")
                    .Sum(t => t.Amount),

                TotalRevenue = transactions
                    .Where(t => t.TransactionStatus == "Success")
                    .Sum(t => t.Amount)
            };
        }

        private static void ValidatePagination(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new BadRequestException("Page number must be greater than 0.");

            if (pageSize <= 0)
                throw new BadRequestException("Page size must be greater than 0.");

            if (pageSize > 100)
                throw new BadRequestException("Page size cannot be greater than 100.");
        }
    }
}