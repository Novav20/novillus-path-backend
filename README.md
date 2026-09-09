# SourceGuild Backend — Enterprise E-Learning & Course Platform API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14.0-239120?style=flat&logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Database](https://img.shields.io/badge/Database-SQL_Server_2022-CC292B?style=flat&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![Docker](https://img.shields.io/badge/Container-Docker-2496ED?style=flat&logo=docker)](https://www.docker.com/)
[![Tests](https://img.shields.io/badge/Tests-20%20Passed-success?style=flat&logo=xunit)](https://xunit.net/)

SourceGuild is a high-performance, domain-driven e-learning backend engineered with **ASP.NET Core**. It provides secure, scalable RESTful APIs for course authoring, hierarchical section/lesson curricula, polymorphic multimedia content delivery, role-based student enrollments, progress tracking, and reviews.

The architecture has been refactored and modernized to strictly enforce **Domain-Driven Design (DDD) Aggregate Boundaries**, **Vertical Slice Feature Handlers**, the **Result Pattern (eliminating exception-based flow control)**, **zero-allocation static compile-time mappings**, and containerized persistence using **Microsoft SQL Server 2022**.

---

## 🏛️ Architecture & Solution Layout

The solution adopts modern enterprise standards, utilizing `.slnx` solution format, Clean Architecture layer boundaries, and Feature-driven organization:

```text
SourceGuild/
├── src/
│   ├── SourceGuild.Domain/          # Pure Domain layer: Rich Aggregate Roots, Value Objects, Enums, Result Pattern
│   ├── SourceGuild.Application/     # Vertical Slices / Feature Handlers, DTOs, FluentValidation, Static Mappings
│   ├── SourceGuild.Infrastructure/  # EF Core DbContext, Split-Query Repositories, Identity JWT, Data Seeders
│   └── SourceGuild.API/             # ASP.NET Core Controllers, RFC 7807 ProblemDetails, OpenAPI/Swagger Docs
├── tests/
│   └── SourceGuild.Tests/           # Automated Test Suite: 20 Unit & Feature Tests (xUnit + Moq + FluentAssertions)
├── docs/                            # Docs-as-Code: PlantUML Domain Models, Logical ERD, C4 Diagrams, Architecture Roadmap
└── scripts/                         # DevOps automation: setup-secrets.sh, run-migrations.sh
```

### Key Architectural Highlights
1. **Rich Aggregate Roots vs. Anemic Domain:**
   * `Course` acts as the strict **Aggregate Root** protecting all consistency boundaries for its child entities (`Section`, `Lesson`, `ContentBlock`). 
   * Mutating child entities, re-indexing orders, or altering lifecycle states (`Draft` $\to$ `Published`) is strictly encapsulated within the aggregate root, eliminating orphan records.
2. **Result Pattern & RFC 7807 ProblemDetails:**
   * Replaced performance-heavy runtime exceptions (`ServiceExceptions`) with an explicit, zero-dependency `Result<T>` and domain `Error` record pattern.
   * `ResultExtensions` dynamically maps domain errors (`Error.NotFound`, `Error.Validation`, `Error.Conflict`) to standard HTTP ProblemDetails (404, 400, 409, 401, 403).
3. **Zero-Allocation Compile-Time Mappings:**
   * Completely eliminated `AutoMapper` and its reflection overhead in favor of static extension methods (`MappingExtensions.ToDto()`) utilizing C# pattern matching for polymorphic content blocks (`TextContent`, `VideoContent`), ensuring full Native AOT readiness.
4. **EF Core Query Optimization:**
   * Configured `QuerySplittingBehavior.SplitQuery` to eliminate Cartesian explosions when querying deep aggregate graphs (`Course` $\to$ `Sections` $\to$ `Lessons` $\to$ `ContentBlocks`).
5. **Security & Secrets Governance:**
   * Zero hardcoded credentials. All connection strings, SA passwords, and JWT private keys are resolved dynamically via `dotnet user-secrets` in development and parameterized `.env` files for Docker.

---

## 🛠️ Technology Stack

| Concern | Technology / Pattern |
| :--- | :--- |
| **Language & Runtime** | C# 14 / .NET 10.0 SDK |
| **Architectural Style** | Domain-Driven Design (DDD) + Vertical Slice Architecture |
| **Web API & Routing** | ASP.NET Core Controllers with OpenAPI / Swagger XML Documentation |
| **Authentication & IAM** | ASP.NET Core Identity + JWT Bearer Tokens (Role-Based Access Control) |
| **ORM & Data Access** | Entity Framework Core |
| **Database Engine** | Microsoft SQL Server 2022 (Linux Container via Docker Compose) |
| **Validation & Error Handling**| FluentValidation + Functional Result Pattern (`Result<T>`, `Error`) |
| **Testing Frameworks** | xUnit, Moq 4.20, FluentAssertions |
| **Documentation & Tooling** | PlantUML, Docker Compose, Shell Automation |

---

## 🚀 Getting Started

### Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet)
* [Docker Engine](https://docs.docker.com/engine/install/) & Docker Compose

### 1. Clone the repository
```bash
git clone https://github.com/Novav20/source-guild-backend.git
cd source-guild-backend
```

### 2. Configure Environment & Start SQL Server
```bash
# Copy the environment template
cp .env.example .env

# Start SQL Server 2022 container on host port 1434 (configured in .env)
docker compose up -d
```

### 3. Initialize User Secrets
```bash
# Automated secrets configuration script
./scripts/setup-secrets.sh
```

### 4. Apply Database Migrations & Seed Data
```bash
# Run automated interactive migration script (Option 1)
./scripts/run-migrations.sh
```
*The database will be created and automatically populated with realistic test data (Instructors, Students, Categories, Courses with full curricula, and Reviews).*

### 5. Run the API
```bash
dotnet run --project src/SourceGuild.API
```
Navigate to `http://localhost:5037/swagger` to explore and execute the interactive OpenAPI specification.

---

## 🧪 Running Automated Tests

Run the complete test suite (Domain Aggregate Invariants & Application Feature Handlers):

```bash
dotnet test
```

---

## 📖 Architecture & Design Documentation

Comprehensive technical diagrams and evolution roadmaps are available in the `docs/` folder:

* [Domain Model Class Diagram (DDD)](docs/domain/domain-model.puml)
* [Logical Entity-Relationship Diagram (ERD)](docs/domain/logical-erd.puml)
* [Course Enrollment Sequence Diagram](docs/domain/course-enrollment-seq.puml)
* [Content Blocks Architectural Roadmap](docs/architecture/content-blocks-roadmap.md)

---

## 📄 License
This project is open-source and available under the [MIT License](LICENSE).