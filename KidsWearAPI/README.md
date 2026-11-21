# KidsWear API – ASP.NET Core Backend

The backend REST API for the KidsWear e-commerce application, built using ASP.NET Core Web API and SQL Server.

## Tech Stack

- ASP.NET Core 10 Web API
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication
- Razorpay Payment Gateway (Test Mode)
- Swagger / OpenAPI

## Features

### User
- User registration and login
- JWT authentication
- Refresh token support
- Product browsing and search
- Cart management
- Wishlist management
- Address management
- Order placement and cancellation
- Cash on Delivery
- Razorpay online payment integration

### Admin
- Admin authentication
- Role-based authorization
- Product management
- User management
- Order management
- Dashboard summary

## Project Structure

```text
KidsWearAPI/
├── Configuration/
├── Controllers/
├── Data/
├── DTOs/
├── Interfaces/
├── Migrations/
├── Models/
├── Services/
├── wwwroot/
│   └── Images/
├── Program.cs
└── appsettings.json
```

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/misnaa-pp/Ecommerce-Backend.git
```

### 2. Navigate to the project

```bash
cd Ecommerce-Backend/KidsWearAPI
```

### 3. Configure the database

Update the `DefaultConnection` connection string using your local SQL Server details.

### 4. Configure application secrets

Set your JWT and Razorpay credentials using .NET User Secrets. Do not commit actual credentials to GitHub.

### 5. Apply database migrations

```bash
dotnet ef database update
```

### 6. Run the application

```bash
dotnet run
```

Open Swagger to explore and test the API endpoints.

## Payment Integration

Razorpay is integrated in Test Mode for online payment processing. Payment verification includes signature validation and server-side payment status checks.

## Related Repository

**Frontend:** KidsWear React application

## Project Purpose

This project was developed to gain practical experience in ASP.NET Core backend development, REST API design, authentication, authorization, Entity Framework Core, SQL Server, and payment gateway integration.