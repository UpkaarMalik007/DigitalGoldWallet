# Digital Gold Wallet

A full-stack fintech application that enables users to buy, sell, and manage digital gold securely. The platform provides role-based access for Admins, Users, and Vendors, ensuring secure transaction management and seamless gold investment operations.

## 📋 Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Role-Based Access](#role-based-access)
- [Security](#security)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## ✨ Features

### User Features
- **Buy Digital Gold**: Purchase digital gold at competitive market rates
- **Sell Digital Gold**: Liquidate holdings with real-time pricing
- **Portfolio Management**: Track gold holdings and transaction history
- **Secure Wallet**: Encrypted storage and management of digital assets
- **Transaction History**: Detailed logs of all buy/sell transactions
- **Real-time Pricing**: Live gold price updates
- **Investment Tracking**: Monitor returns and portfolio performance

### Admin Features
- **User Management**: Create, edit, and manage user accounts
- **Transaction Monitoring**: View and audit all platform transactions
- **Vendor Management**: Manage vendor access and permissions
- **System Analytics**: Dashboard with key performance metrics
- **Security Controls**: Configure security policies and access levels
- **Report Generation**: Generate detailed reports on platform activity

### Vendor Features
- **Inventory Management**: Manage digital gold inventory
- **Price Setting**: Update and manage gold prices
- **Transaction Processing**: Process buy/sell requests
- **Performance Analytics**: Track vendor metrics and sales

## 🛠 Tech Stack

### Backend
- **Language**: C# (60.6%)
- **Framework**: ASP.NET / ASP.NET Core
- **Architecture**: MVC/MVVM pattern with role-based authorization

### Frontend
- **HTML**: 37.4%
- **CSS**: 0.7%
- **Markup & Styling**: Bootstrap or custom CSS framework

### DevOps & Scripting
- **PowerShell**: 1.3%
- **Build & Deployment**: PowerShell scripts for automated builds and deployments

### Database
- SQL Server (recommended for C# applications)

## 🏗 Architecture

```
DigitalGoldWallet/
├── Backend/
│   ├── API/                 # REST API endpoints
│   ├── Services/            # Business logic
│   ├── Models/              # Data models
│   ├── Controllers/         # API controllers
│   └── Middleware/          # Authentication & Authorization
├── Frontend/
│   ├── Views/               # HTML templates
│   ├── Styles/              # CSS stylesheets
│   └── Scripts/             # Client-side logic
├── Database/
│   └── Migrations/          # Database schema migrations
└── Deployment/
    └── Scripts/             # PowerShell deployment scripts
```

## 📦 Prerequisites

- **.NET Framework** 4.7.2 or higher / **.NET Core** 3.1 or higher
- **Visual Studio** 2019 or later / **Visual Studio Code**
- **SQL Server** 2016 or higher
- **PowerShell** 5.0 or higher
- **NuGet** Package Manager

## 🚀 Installation

### 1. Clone the Repository
```bash
git clone https://github.com/UpkaarMalik007/DigitalGoldWallet.git
cd DigitalGoldWallet
```

### 2. Restore NuGet Packages
```bash
dotnet restore
```

### 3. Database Setup
```bash
# Run migrations
dotnet ef database update
```

### 4. Build the Project
```bash
dotnet build
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001` (or configured port)

## ⚙️ Configuration

### appsettings.json
Create an `appsettings.json` file in the root directory:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DigitalGoldWallet;User Id=sa;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "DigitalGoldWallet",
    "Audience": "DigitalGoldWalletUsers"
  },
  "GoldPriceApi": {
    "Endpoint": "https://api.example.com/gold-price",
    "ApiKey": "your_api_key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## 💡 Usage

### Starting the Application

#### Development
```bash
dotnet run --configuration Development
```

#### Production
```bash
dotnet publish -c Release
dotnet DigitalGoldWallet.dll
```

### Sample Workflow

1. **Register**: Create a new user account
2. **Login**: Authenticate with credentials
3. **View Portfolio**: Check your gold holdings
4. **Buy Gold**: Select amount and execute purchase
5. **View History**: Review all transactions
6. **Sell Gold**: Liquidate holdings when desired

## 📁 Project Structure

```
DigitalGoldWallet/
├── src/
│   ├── DigitalGoldWallet.API/       # API project (C#)
│   ├── DigitalGoldWallet.Core/      # Core business logic (C#)
│   ├── DigitalGoldWallet.Data/      # Data access layer (C#)
│   └── DigitalGoldWallet.Models/    # Data models (C#)
├── wwwroot/
│   ├── css/                          # Stylesheets
│   ├── js/                           # Client-side scripts
│   └── images/                       # Static images
├── Views/                            # HTML templates
├── Deployment/
│   └── deploy.ps1                    # PowerShell deployment script
├── appsettings.json
├── README.md
└── .gitignore
```

## 🔐 Role-Based Access

### Admin
- Full platform access
- User and vendor management
- Transaction monitoring
- System configuration

### User
- Personal portfolio management
- Buy/sell gold
- View transaction history
- Profile management

### Vendor
- Inventory management
- Price updates
- Transaction processing
- Performance metrics

Access control is enforced through:
- JWT token authentication
- Role-based authorization policies
- Middleware-level permission checks

## 🛡 Security

- **Authentication**: JWT-based token authentication
- **Authorization**: Role-based access control (RBAC)
- **Encryption**: HTTPS/TLS for data in transit
- **Data Protection**: Encrypted sensitive data at rest
- **Input Validation**: Server-side validation on all inputs
- **CSRF Protection**: Anti-forgery tokens for state-changing operations
- **SQL Injection Prevention**: Parameterized queries using Entity Framework
- **Audit Logging**: All transactions logged for compliance

## 🔌 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout
- `POST /api/auth/refresh-token` - Refresh JWT token

### Gold Transactions
- `GET /api/gold/price` - Get current gold price
- `POST /api/gold/buy` - Purchase digital gold
- `POST /api/gold/sell` - Sell digital gold
- `GET /api/gold/portfolio` - Get user portfolio
- `GET /api/gold/history` - Get transaction history

### User Management (Admin)
- `GET /api/admin/users` - List all users
- `POST /api/admin/users` - Create new user
- `PUT /api/admin/users/{id}` - Update user
- `DELETE /api/admin/users/{id}` - Delete user

### Vendor Management
- `GET /api/vendor/inventory` - Get inventory
- `PUT /api/vendor/price` - Update gold price
- `GET /api/vendor/transactions` - View transactions

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style Guidelines
- Follow C# naming conventions (PascalCase for public members)
- Write unit tests for new features
- Ensure all tests pass before submitting PR
- Update documentation as needed

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📧 Contact

For questions or support, please contact:
- **GitHub**: [@UpkaarMalik007](https://github.com/UpkaarMalik007)

## 🙏 Acknowledgments

- Thanks to all contributors and community members
- Built with ASP.NET Core and modern web technologies
- Inspired by secure fintech practices

---

**Last Updated**: June 2026

Happy investing in digital gold! 🏆
