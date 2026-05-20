# Loan Management System - Layered Architecture Demo

This project is a .NET Web API demonstrating a strict, domain-centric **5-Tier Layered Architecture**. It handles complex business workflows for loan origination, risk assessment, and fund disbursement.

## 🏗 Architecture Overview

Unlike traditional N-Tier architectures where dependencies flow top-down to the database, this solution uses **Dependency Inversion** to keep the core business logic completely isolated from infrastructure concerns. 

Dependencies flow strictly inward toward the `DomainCore` and `Business` layers.

### Project Structure & Dependency Flow

```text
Demo.Solution
│
├── 1. Demo.DomainCore (Entities, Enums, I/O Interfaces)
│   └── Dependencies: NONE.
│
├── 2. Demo.DataAccess (EF Core, API Clients)
│   └── Dependencies: Demo.DomainCore.
│
├── 3. Demo.Business (Pure Logic, Risk Assessment)
│   └── Dependencies: Demo.DomainCore.
│
├── 4. Demo.Application (Use Cases, Orchestration)
│   └── Dependencies: Demo.Business, Demo.DomainCore.
│
└── 5. Demo.Presentation (Web API, Controllers)
    └── Dependencies: Demo.Application. (Also acts as Composition Root)
```

## 🧠 Key Design Decisions

* **Separation of Application vs. Business Layers:** * The `Application` layer (`Demo.Application`) handles I/O orchestration (fetching from DBs, calling external APIs). 
  * The `Business` layer (`Demo.Business`) handles pure C# math and state validation (e.g., Debt-to-Income calculation, tier-based interest rates). This allows for lightning-fast, sociable unit testing without mocking databases.
* **Pragmatic Dependency Injection:** * Interfaces (`ILoanRepository`, `ICreditBureauClient`) are strictly used for out-of-process I/O boundaries to enable mocking.
  * Concrete classes (`ProcessLoanUseCase`, `RiskAssessmentService`) are injected directly where polymorphic behavior or network mocking is unnecessary, reducing interface bloat.
* **Composition Root:** `Demo.Presentation/Program.cs` is the only file allowed to reference all layers, purely to configure the IoC container.

## 🚀 Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)
* An IDE of your choice (Visual Studio, JetBrains Rider, VS Code)

### Running the Application

1. Clone the repository.
2. Navigate to the Presentation project:
   ```bash
   cd Demo.Presentation
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Navigate to `https://localhost:<port>/swagger` (or `/scalar` if configured) to view the API documentation.

## 📡 API Endpoints

### 1. Apply for a Loan
Evaluates a customer's credit score and financial standing to approve or reject a loan application.

* **URL:** `/api/loans/apply`
* **Method:** `POST`
* **Payload:**
  ```json
  {
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "amount": 50000.00,
    "monthlyIncome": 8500.00,
    "existingDebt": 1200.00
  }
  ```

### 2. Disburse Funds
Calculates origination fees and wires the final payout amount to an approved applicant.

* **URL:** `/api/loans/{id}/disburse`
* **Method:** `POST`
* **Response:** Returns `200 OK` on successful bank transfer, or `502 Bad Gateway` if the external banking provider fails.

## 🛠 Simulated External Dependencies

This demo utilizes fake HTTP clients in the Data Access layer to simulate microservice/3rd-party integration:
* **Credit Bureau API:** Simulates fetching live credit scores during the application phase.
* **Banking Partner API:** Simulates wire transfers during the disbursement phase.
