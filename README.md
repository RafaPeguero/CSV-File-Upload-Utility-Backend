## Backend README

```markdown
# Sales Order Upload Utility - Backend

A .NET Web API backend for processing and storing sales order data from CSV and Excel files, with an in-memory Cosmos DB implementation.

## Features

- File upload API for CSV and Excel files
- Robust file parsing and data extraction
- Data validation with detailed error reporting
- In-memory Cosmos DB implementation (can be replaced with real Cosmos DB)
- RESTful API design
- Clean architecture with service abstractions

## Technologies Used

- .NET 6+
- ASP.NET Core Web API
- Entity Framework Core
- CsvHelper for CSV parsing
- ExcelDataReader for Excel parsing
- Cosmos DB
- MSTest for unit testing

## Project Setup

### Prerequisites

- .NET 6 SDK or higher
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/sales-order-upload-backend.git
   cd sales-order-upload-backend

2. Restore Dependencies
   ```bash
   dotnet restore

3. Run th app and you will get the swagger page with the endpoints
