# 🏠 House Broker App

A **Clean Architecture .NET 9 MVC + API application** for managing property listings, brokers, and seekers.  
Supports **cookie-based authentication (ASP.NET Identity)** for MVC and **JWT authentication** for APIs.

---

## 📌 Features

- **User Authentication**
  - ASP.NET Identity with cookie-based login (MVC).
  - JWT-based login for API endpoints.
- **Property Listings**
  - CRUD operations for brokers.
  - Search by filters (location, price, type).
  - Commission calculation engine with DB-configurable rules.
- **Broker Dashboard**
  - View own listings.
  - Total commission summary.
- **API Support**
  - Listings API (CRUD + search).
  - Account API (Register/Login with JWT).
  - Broker API (Commission + Listings).
- **Caching**
  - Memory cache for frequently accessed listings.
- **Validation**
  - FluentValidation for DTOs.
- **Database**
  - MS SQL Server with EF Core.
- **Testing**
  - Unit tests (xUnit + Moq).
  - Covers services, controllers, and API endpoints.
- **Documentation**
  - Swagger/OpenAPI enabled.

---

## 🏗️ Architecture

The solution follows **Clean Architecture**:

```
├── HouseBrokerApp.Core          # Entities, Interfaces
├── HouseBrokerApp.Application   # Services, DTOs, Validators
├── HouseBrokerApp.Infrastructure# EF Core, Repositories, Identity, JWT
├── HouseBrokerApp.Web           # MVC + API Controllers
└── HouseBrokerApp.Tests         # Unit tests
```

---

## ⚙️ Setup

### 1️⃣ Clone Repository
```bash
git clone https://github.com/amanshakya017/house-broker-application.git
cd HouseBrokerApp
```

### 2️⃣ Configure Database Connection & JWT
In `appsettings.json`, set your **Connection String** and **JWT**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HouseBrokerDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
},
"Jwt": {
  "Key": "your_very_long_secure_base64_key_here",
  "Issuer": "HouseBrokerApp",
  "Audience": "HouseBrokerApp",
  "ExpireHours": 2
}
```

📌 Ensure:
- Update **Server**, **User Id**, and **Password** with your SQL Server credentials.
- `TrustServerCertificate=True;` is useful for local development.

### 3️⃣ Update Database
```bash
dotnet ef database update --project HouseBrokerApp.Infrastructure --startup-project HouseBrokerApp.Web
```

### 4️⃣ Run Application
```bash
dotnet run --project HouseBrokerApp.Web
```

### 5️⃣ Default Users
- **Broker**  
  Username: `broker`  
  Password: `P@ssw0rd`

- **Seeker**  
  Username: `seeker`  
  Password: `P@ssw0rd`

---

## 🧪 Testing

Run all tests from the root folder:

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 📚 API Documentation

After running the app, visit Swagger UI:

👉 [https://localhost:5081/swagger](https://localhost:5081/swagger)

---

## 🛠️ Tech Stack

- .NET 9 / ASP.NET Core MVC + API
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- FluentValidation
- Swagger / OpenAPI
- xUnit + Moq

---