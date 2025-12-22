# SourceGuild

## Project Overview

SourceGuild is an online learning platform backend built with ASP.NET Core. It provides APIs for managing courses, sections, lessons, content blocks, user authentication, enrollments, and reviews. The project follows Clean Architecture principles and uses Entity Framework Core for data persistence.

## Setup Instructions

### Prerequisites

- .NET SDK 9.0 (or compatible version)
- SQL Server (or Docker Desktop with SQL Server image)
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/Novav20/SourceGuild.git
cd SourceGuild
```

### 2. Configure Database Connection String

Update the `ConnectionStrings` in `SourceGuild.API/appsettings.Development.json` to point to your SQL Server instance. For local development, it's recommended to use User Secrets for sensitive information like database credentials.

Example `appsettings.Development.json` (ensure `DbCredentials:UserId` and `DbCredentials:Password` are set in User Secrets):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
    "ConnectionStrings": {
    "SourceGuildDbConnection_Template": "Data Source=localhost,1433;Database=SourceGuildDb;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Connect Retry Count=1;Connect Retry Interval=10;Command Timeout=30"
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
    "Issuer": "SourceGuild.API",
    "Audience": "SourceGuild.UI",
    "DurationInMinutes": 60
  },
  "SeedAdminCredentials": {
    "Email": "admin@sourceguild.com",
    "Password": "Admin123!"
  }
}
```

To set User Secrets for `DbCredentials` and `JwtSettings:SecretKey`:

```bash
dotnet user-secrets set "DbCredentials:UserId" "your_db_user"
dotnet user-secrets set "DbCredentials:Password" "your_db_password"
dotnet user-secrets set "JwtSettings:SecretKey" "YourSuperSecretKeyThatIsAtLeast32CharactersLong"
```

### 3. Run Database Migrations

Navigate to the project root and run the migrations. This will create the database schema and seed essential data (roles, admin user).

```bash
dotnet ef database update --project SourceGuild.Infrastructure --startup-project SourceGuild.API
```

### 4. Run the API

```bash
dotnet run --project SourceGuild.API
```

The API will typically run on `https://localhost:7000` (or a similar port). You can find the exact URL in `SourceGuild.API/Properties/launchSettings.json`.

## API Usage

Once the API is running, you can access the Swagger UI at `https://localhost:7000/swagger` (adjust port if necessary) to explore the available endpoints and test them.

### Admin User Credentials (for development seeding):
- **Email:** `admin@sourceguild.com`
- **Password:** `Admin123!`

### Example Endpoints:

- **Register User:** `POST /api/auth/register`
- **Login User:** `POST /api/auth/login`
- **Get Courses:** `GET /api/courses`
- **Create Course:** `POST /api/courses` (requires Admin/Instructor role)
- **Get Student Dashboard:** `GET /api/dashboard/student` (requires Student role)

Refer to the Swagger UI for detailed information on all endpoints, request/response models, and authorization requirements.