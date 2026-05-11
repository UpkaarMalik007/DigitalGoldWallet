using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
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

        public async Task BuyGold(GoldActionRequestDto dto)
        {
            var user = await _goldRepository
                .GetUserById(dto.UserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.Balance < dto.Amount)
            {
                throw new Exception(
                    "Insufficient wallet balance");
            }

            var branch = await _goldRepository
                .GetBranchById(dto.BranchId.Value);

            if (branch == null)
            {
                throw new Exception("Branch not found");
            }

            if (branch.Vendor == null)
            {
                throw new Exception("Vendor not found");
            }

            decimal goldPrice =
                branch.Vendor.CurrentGoldPrice;

            decimal quantity =
                dto.Amount.Value / goldPrice;


            user.Balance -= dto.Amount.Value;


            var holding = await _goldRepository
                .GetHolding(dto.UserId);

            if (holding == null)
            {
                holding = new VirtualGoldHolding
                {
                    UserId = dto.UserId,
                    BranchId = dto.BranchId.Value,
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

            var transaction =
                new TransactionHistory
                {
                    UserId = dto.UserId,
                    BranchId = dto.BranchId.Value,
                    TransactionType = "Buy",
                    TransactionStatus = "Success",
                    Quantity = quantity,
                    Amount = dto.Amount.Value,
                    CreatedAt = DateTime.Now
                };

            await _goldRepository
                .AddTransactionHistory(
                    transaction);

            await _goldRepository
                .SaveChanges();
        }
        public async Task SellGold(GoldActionRequestDto dto)
        {
            var user = await _goldRepository
                .GetUserById(dto.UserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var holding = await _goldRepository
                .GetHolding(dto.UserId);

            if (holding == null ||
                holding.Quantity < dto.Quantity.Value)
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
                dto.Quantity.Value * goldPrice;

            holding.Quantity -= dto.Quantity.Value;

            user.Balance += amount;

            var transaction =
                new TransactionHistory
                {
                    UserId = dto.UserId,
                    BranchId = holding.BranchId,
                    TransactionType = "Sell",
                    TransactionStatus = "Success",
                    Quantity = dto.Quantity.Value,
                    Amount = amount,
                    CreatedAt = DateTime.Now
                };

            await _goldRepository
                .AddTransactionHistory(
                    transaction);

            await _goldRepository
                .SaveChanges();
        }

        public async Task<GoldPortfolioDto> GetHoldings(int userId)
        {
            var holding = await _goldRepository
                .GetHolding(userId);

            return new GoldPortfolioDto
            {
                UserId = userId,
                TotalGold = holding?.Quantity ?? 0,
                CurrentGoldPrice = await _goldRepository.GetCurrentGoldPrice(),
                GoldPriceUpdatedAt = DateTime.Now,
                CurrentValue = (holding?.Quantity ?? 0) * await _goldRepository.GetCurrentGoldPrice(),
                TotalInvestment = 0, 
                ProfitLoss = 0
            };
        }

        public async Task<GoldPortfolioDto> GetCurrentPrice()
        {
            decimal price =
                await _goldRepository
                    .GetCurrentGoldPrice();

            return new GoldPortfolioDto
            {
                CurrentGoldPrice = price,
                GoldPriceUpdatedAt = DateTime.Now
            };
        }

        public async Task ConvertToPhysical(GoldActionRequestDto dto)
        {
            var holding = await _goldRepository
                .GetHolding(dto.UserId);

            if (holding == null ||
                holding.Quantity < dto.Quantity.Value)
            {
                throw new Exception(
                    "Insufficient gold quantity");
            }

            holding.Quantity -= dto.Quantity.Value;

            var transaction =
                new PhysicalGoldTransaction
                {
                    UserId = dto.UserId,
                    BranchId = dto.BranchId.Value,
                    Quantity = dto.Quantity.Value,
                    DeliveryAddressId = dto.DeliveryAddressId.Value,
                    CreatedAt = DateTime.Now
                };

            await _goldRepository
                .AddPhysicalTransaction(
                    transaction);

            await _goldRepository
                .SaveChanges();
        }


        public async Task<List<GoldTransactionDto>> GetPhysicalHistory(int userId)
        {
            var data = await _goldRepository
                .GetPhysicalTransactions(userId);

            return _mapper.Map<List<GoldTransactionDto>>(data);
        }


        public async Task<List<GoldTransactionDto>>
            GetTransactions(int userId)
        {
            var data = await _goldRepository
                .GetTransactions(userId);

            return _mapper.Map<List<GoldTransactionDto>>(data);
        }

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