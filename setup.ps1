# ========================================
# DigitalGoldWallet Setup Script
# macOS + VS Code Compatible
# Run:
# pwsh ./setup.ps1
# ========================================

$ProjectName = "DigitalGoldWallet"

function Ensure-Directory {
    param([string]$Path)
    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}

function Ensure-File {
    param([string]$Path)
    New-Item -ItemType File -Path $Path -Force | Out-Null
}

# ========================================
# ROOT PROJECT
# ========================================

if (Test-Path $ProjectName) {
    Set-Location $ProjectName
}
else {
    Ensure-Directory $ProjectName
    Set-Location $ProjectName
}
# ========================================
# ROOT LEVEL FOLDERS
# ========================================

Ensure-Directory ".github/workflows"
Ensure-Directory ".vscode"
Ensure-Directory ".vs"

# ========================================
# ROOT FILES
# ========================================

Ensure-File "README.md"
Ensure-File ".gitignore"
Ensure-File "DigitalGoldWalletDB.sql"

# ========================================
# DOCS
# ========================================

Ensure-Directory "docs"

Ensure-File "docs/README.md"
Ensure-File "docs/API-Contracts.md"
Ensure-File "docs/Database-Schema.md"
Ensure-File "docs/SoftDelete-Migration.sql"

# ========================================
# CREATE SOLUTION
# ========================================

dotnet new sln -n DigitalGoldWallet --force | Out-Null

# ========================================
# CREATE PROJECTS
# ========================================

dotnet new webapi -n DigitalGoldWallet.API -o DigitalGoldWallet.API --force | Out-Null
dotnet new mvc -n DigitalGoldWallet.MVC -o DigitalGoldWallet.MVC --force | Out-Null
dotnet new xunit -n DigitalGoldWallet.Tests -o DigitalGoldWallet.Tests --force | Out-Null

# ========================================
# ADD PROJECTS TO SOLUTION
# ========================================

dotnet sln add "DigitalGoldWallet.API/DigitalGoldWallet.API.csproj" | Out-Null
dotnet sln add "DigitalGoldWallet.MVC/DigitalGoldWallet.MVC.csproj" | Out-Null
dotnet sln add "DigitalGoldWallet.Tests/DigitalGoldWallet.Tests.csproj" | Out-Null

# ========================================
# API PROJECT
# ========================================

Push-Location "DigitalGoldWallet.API"

$ApiFolders = @(
    "Configuration",
    "Controllers",
    "DTOs",
    "Exceptions",
    "Helpers",
    "Middleware",
    "Models",
    "Repositories",
    "Repositories/Interfaces",
    "Repositories/Implementations",
    "Services",
    "Services/Interfaces",
    "Services/Implementations",
    "Validators"
)

foreach ($folder in $ApiFolders) {
    Ensure-Directory $folder
}

$ApiFiles = @(

    # Controllers
    "Controllers/AuthController.cs",
    "Controllers/UserController.cs",
    "Controllers/WalletController.cs",
    "Controllers/GoldController.cs",
    "Controllers/VendorController.cs",
    "Controllers/TransactionController.cs",

    # Models
    "Models/User.cs",
    "Models/Address.cs",
    "Models/Vendor.cs",
    "Models/VendorBranch.cs",
    "Models/Payment.cs",
    "Models/VirtualGoldHolding.cs",
    "Models/PhysicalGoldTransaction.cs",
    "Models/TransactionHistory.cs",
    "Models/AppDbContext.cs",

    # DTOs
    "DTOs/AuthDtos.cs",
    "DTOs/UserDtos.cs",
    "DTOs/WalletDtos.cs",
    "DTOs/GoldDtos.cs",
    "DTOs/VendorDtos.cs",
    "DTOs/TransactionDtos.cs",

    # Repository Interfaces
    "Repositories/Interfaces/IUserRepository.cs",
    "Repositories/Interfaces/IWalletRepository.cs",
    "Repositories/Interfaces/IGoldRepository.cs",
    "Repositories/Interfaces/IVendorRepository.cs",
    "Repositories/Interfaces/ITransactionRepository.cs",

    # Repository Implementations
    "Repositories/Implementations/UserRepository.cs",
    "Repositories/Implementations/WalletRepository.cs",
    "Repositories/Implementations/GoldRepository.cs",
    "Repositories/Implementations/VendorRepository.cs",
    "Repositories/Implementations/TransactionRepository.cs",

    # Service Interfaces
    "Services/Interfaces/IAuthService.cs",
    "Services/Interfaces/IUserService.cs",
    "Services/Interfaces/IWalletService.cs",
    "Services/Interfaces/IGoldService.cs",
    "Services/Interfaces/IVendorService.cs",
    "Services/Interfaces/ITransactionService.cs",

    # Service Implementations
    "Services/Implementations/AuthService.cs",
    "Services/Implementations/UserService.cs",
    "Services/Implementations/WalletService.cs",
    "Services/Implementations/GoldService.cs",
    "Services/Implementations/VendorService.cs",
    "Services/Implementations/TransactionService.cs",

    # Middleware
    "Middleware/JwtMiddleware.cs",
    "Middleware/GlobalExceptionMiddleware.cs",

    # Helpers
    "Helpers/JwtHelper.cs",
    "Helpers/PasswordHelper.cs",
    "Helpers/SoftDeleteQueryExtensions.cs",

    # Exceptions
    "Exceptions/BadRequestException.cs",
    "Exceptions/NotFoundException.cs",
    "Exceptions/ForbiddenException.cs",

    # Validators
    "Validators/RegisterValidator.cs",
    "Validators/LoginValidator.cs",
    "Validators/WalletValidator.cs",
    "Validators/GoldValidator.cs",

    # Configuration
    "Configuration/JwtSettings.cs",
    "Configuration/AppSettings.cs"
)

foreach ($file in $ApiFiles) {
    Ensure-File $file
}

Pop-Location

# ========================================
# MVC PROJECT
# ========================================

Push-Location "DigitalGoldWallet.MVC"

$MvcFolders = @(
    "Controllers",
    "Models",
    "Views/Shared",
    "Views/Account",
    "Views/User",
    "Views/Wallet",
    "Views/Gold",
    "Views/Vendor",
    "Views/Transaction",
    "wwwroot/css",
    "wwwroot/js",
    "wwwroot/images",
    "wwwroot/uploads"
)

foreach ($folder in $MvcFolders) {
    Ensure-Directory $folder
}

$MvcFiles = @(

    # Shared
    "Views/Shared/_Layout.cshtml",
    "Views/Shared/Error.cshtml",

    # Account
    "Views/Account/Login.cshtml",
    "Views/Account/Register.cshtml",

    # User
    "Views/User/Profile.cshtml",
    "Views/User/Addresses.cshtml",

    # Wallet
    "Views/Wallet/AddMoney.cshtml",
    "Views/Wallet/History.cshtml",

    # Gold
    "Views/Gold/BuyGold.cshtml",
    "Views/Gold/SellGold.cshtml",
    "Views/Gold/Holdings.cshtml",

    # Vendor
    "Views/Vendor/List.cshtml",
    "Views/Vendor/Details.cshtml",

    # Transaction
    "Views/Transaction/History.cshtml",

    # CSS & JS
    "wwwroot/css/site.css",
    "wwwroot/js/site.js"
)

foreach ($file in $MvcFiles) {
    Ensure-File $file
}

Pop-Location

# ========================================
# TEST PROJECT
# ========================================

Push-Location "DigitalGoldWallet.Tests"

$TestFolders = @(
    "UnitTests/Services",
    "IntegrationTests",
    "Helpers"
)

foreach ($folder in $TestFolders) {
    Ensure-Directory $folder
}

$TestFiles = @(

    # Unit Tests
    "UnitTests/Services/AuthServiceTests.cs",
    "UnitTests/Services/UserServiceTests.cs",
    "UnitTests/Services/WalletServiceTests.cs",
    "UnitTests/Services/GoldServiceTests.cs",
    "UnitTests/Services/VendorServiceTests.cs",
    "UnitTests/Services/TransactionServiceTests.cs",

    # Integration Tests
    "IntegrationTests/AuthFlowTests.cs",
    "IntegrationTests/WalletFlowTests.cs",
    "IntegrationTests/GoldFlowTests.cs",

    # Helpers
    "Helpers/TestDataFactory.cs"
)

foreach ($file in $TestFiles) {
    Ensure-File $file
}

Pop-Location

# ========================================
# FINAL MESSAGE
# ========================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "DigitalGoldWallet Solution Created Successfully" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Open DigitalGoldWallet.sln in VS Code" -ForegroundColor Green
Write-Host ""