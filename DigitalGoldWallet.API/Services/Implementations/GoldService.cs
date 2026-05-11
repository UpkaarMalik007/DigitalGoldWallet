using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.API.Exceptions;
using AutoMapper;

namespace DigitalGoldWallet.API.Services.Implementations
{
    public class GoldService : IGoldService
    {
        private readonly IGoldRepository _goldRepository;
        private readonly IMapper _mapper;

        public GoldService(
            IGoldRepository goldRepository,
            IMapper mapper)
        {
            _goldRepository = goldRepository;
            _mapper = mapper;
        }

        // BUY GOLD

        public async Task BuyGold(BuyGoldDto dto)
        {
            var user = await _goldRepository
                .GetUserById(dto.UserId);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (user.Balance < dto.Amount)
            {
                throw new Exception(
                    "Insufficient wallet balance");
            }

            var branch = await _goldRepository
                .GetBranchById(dto.BranchId);

            if (branch == null)
            {
                throw new NotFoundException("Branch not found");
            }

            if (branch.Vendor == null)
            {
                throw new NotFoundException("Vendor not found");
            }

            decimal goldPrice =
                branch.Vendor.CurrentGoldPrice;

            decimal quantity =
                dto.Amount / goldPrice;

            // Deduct wallet balance

            user.Balance -= dto.Amount;

            // Get holding

            var holding = await _goldRepository
                .GetHolding(dto.UserId);

            if (holding == null)
            {
                holding = new VirtualGoldHolding
                {
                    UserId = dto.UserId,
                    BranchId = dto.BranchId,
                    Quantity = quantity,
                    CreatedAt = DateTime.Now
                };

                await _goldRepository
                    .AddHolding(holding);
            }
            else
            {
                holding.Quantity += quantity;
            }

            // Add transaction

            var transaction =
                new TransactionHistory
                {
                    UserId = dto.UserId,
                    BranchId = dto.BranchId,
                    TransactionType = "Buy",
                    TransactionStatus = "Success",
                    Quantity = quantity,
                    Amount = dto.Amount,
                    CreatedAt = DateTime.Now
                };

            await _goldRepository
                .AddTransactionHistory(
                    transaction);

            await _goldRepository
                .SaveChanges();
        }

        // SELL GOLD
        public async Task SellGold(
            SellGoldDto dto)
        {
            var user = await _goldRepository
                .GetUserById(dto.UserId);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var holding = await _goldRepository
                .GetHolding(dto.UserId);

            if (holding == null ||
                holding.Quantity < dto.Quantity)
            {
                throw new Exception(
                    "Insufficient gold quantity");
            }

            var branch = await _goldRepository
                .GetBranchById(
                    holding.BranchId ?? 0);

            decimal goldPrice =
                branch?.Vendor?.CurrentGoldPrice ?? 0;

            decimal amount =
                dto.Quantity * goldPrice;

            // Reduce holding

            holding.Quantity -= dto.Quantity;

            // Add wallet balance

            user.Balance += amount;

            // Add transaction

            var transaction =
                new TransactionHistory
                {
                    UserId = dto.UserId,
                    BranchId = holding.BranchId,
                    TransactionType = "Sell",
                    TransactionStatus = "Success",
                    Quantity = dto.Quantity,
                    Amount = amount,
                    CreatedAt = DateTime.Now
                };

            await _goldRepository
                .AddTransactionHistory(
                    transaction);

            await _goldRepository
                .SaveChanges();
        }

        // GET HOLDINGS

        public async Task<GoldHoldingDto>
            GetHoldings(int userId)
        {
            var holding = await _goldRepository
                .GetHolding(userId);

            return new GoldHoldingDto
            {
                UserId = userId,

                TotalGoldQuantity =
                    holding?.Quantity ?? 0
            };
        }

        // GET CURRENT PRICE

        public async Task<GoldPriceDto>
            GetCurrentPrice()
        {
            decimal price =
                await _goldRepository
                    .GetCurrentGoldPrice();

            return new GoldPriceDto
            {
                CurrentGoldPrice = price,

                UpdatedAt = DateTime.Now
            };
        }

        // CONVERT TO PHYSICAL

        public async Task ConvertToPhysical(
            ConvertToPhysicalDto dto)
        {
            var holding = await _goldRepository
                .GetHolding(dto.UserId);

            if (holding == null ||
                holding.Quantity < dto.Quantity)
            {
                throw new Exception(
                    "Insufficient gold quantity");
            }

            // Reduce holdings

            holding.Quantity -= dto.Quantity;

            // Add physical transaction

            var transaction =
                new PhysicalGoldTransaction
                {
                    UserId = dto.UserId,
                    BranchId = dto.BranchId,
                    Quantity = dto.Quantity,
                    DeliveryAddressId =
                        dto.DeliveryAddressId,
                    CreatedAt = DateTime.Now
                };

            await _goldRepository
                .AddPhysicalTransaction(
                    transaction);

            await _goldRepository
                .SaveChanges();
        }

        // GET PHYSICAL HISTORY

        public async Task<List<PhysicalGoldHistoryDto>>
            GetPhysicalHistory(int userId)
        {
            var data = await _goldRepository
                .GetPhysicalTransactions(userId);

            return _mapper.Map<List<PhysicalGoldHistoryDto>>(data);
        }

        // GET TRANSACTIONS

        public async Task<List<GoldTransactionDto>>
            GetTransactions(int userId)
        {
            var data = await _goldRepository
                .GetTransactions(userId);

            return _mapper.Map<List<GoldTransactionDto>>(data);
        }

        // GET VENDOR STOCK

        public async Task<VendorStockDto>
            GetVendorStock(int branchId)
        {
            var branch = await _goldRepository
                .GetVendorStock(branchId);

            if (branch == null)
            {
                throw new Exception("Branch not found");
            }

            return _mapper.Map<VendorStockDto>(branch);
        }

        // CALCULATE GOLD

        public async Task<GoldCalculationDto>
            CalculateGold(decimal amount)
        {
            decimal goldPrice =
                await _goldRepository
                    .GetCurrentGoldPrice();

            decimal quantity =
                amount / goldPrice;

            return new GoldCalculationDto
            {
                Amount = amount,

                GoldPrice = goldPrice,

                Quantity = quantity
            };
        }

        // GET PORTFOLIO

        public async Task<GoldPortfolioDto>
            GetPortfolio(int userId)
        {
            var holdings = await _goldRepository
                .GetPortfolio(userId);

            decimal totalGold =
                holdings.Sum(x => x.Quantity);

            decimal currentPrice =
                await _goldRepository
                    .GetCurrentGoldPrice();

            decimal currentValue =
                totalGold * currentPrice;

            return new GoldPortfolioDto
            {
                UserId = userId,

                TotalGold = totalGold,

                CurrentGoldPrice =
                    currentPrice,

                CurrentValue =
                    currentValue,

                TotalInvestment =
                    currentValue,

                ProfitLoss = 0
            };
        }
    }
}