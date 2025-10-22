# EnterpriseTemplate – Enterprise Microservice

A production-ready .NET 9.0 microservice implementing Clean Architecture with CQRS, messaging, and comprehensive observability. Features switchable **Azure** or **OnPrem** infrastructure implementations for cloud portability.

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose
- PowerShell (for load testing scripts)

### 1. Start Infrastructure

Start all supporting services (PostgreSQL, RabbitMQ, Redis, Jaeger, MailHog, PgAdmin):

```bash
docker compose up -d
```

### 2. Run the API

Start the API server with development settings:

```bash
dotnet run --project src/Api -- --urls http://localhost:5100 --environment Development
```

### 3. Verify Setup

Test the endpoints:

- Health checks: `GET http://localhost:5100/health/live`
- List todos: `GET http://localhost:5100/todos`
- Create todo: `POST http://localhost:5100/todos` with `{ "title": "Test todo" }`
- Test messaging: `POST http://localhost:5100/bus/ping`
- Test tracing: `GET http://localhost:5100/test/external`

## 📁 Project Structure

- **src/Api** – ASP.NET Core Web API (presentation layer)
- **src/Application** – CQRS with MediatR, validation, authorization behaviors
- **src/Domain** – Pure business logic, entities, value objects
- **src/Infrastructure** – Data access, messaging, external service adapters
- **src/Worker** – Background service processing
- **docs** – Architecture documentation and references

## 🏗️ Architecture

This project implements Clean Architecture with:

- **CQRS** via MediatR for command/query separation
- **Ports & Adapters** pattern for infrastructure abstraction
- **Policy-based authorization** with custom business rules
- **Event-driven messaging** with MassTransit
- **Comprehensive observability** with OpenTelemetry

See `docs/ARCHITECTURE.md` for detailed design documentation.

## 🛠️ Technology Stack

### Core Framework

- **.NET 9.0** / ASP.NET Core 9.0
- **Entity Framework Core 9.0** with PostgreSQL
- **MediatR 12.4.1** for CQRS implementation
- **FluentValidation 11.10.0** for input validation

### Infrastructure & Services

- **PostgreSQL 16** - Primary database
- **RabbitMQ 3** - Message broker with management UI
- **Redis 7** - Distributed caching
- **PgAdmin 4** - Database management web interface

### Observability

- **OpenTelemetry 1.9.0** - Distributed tracing and metrics
- **Jaeger** - Trace analysis and visualization
- **Serilog** - Structured logging with request correlation

### Development Tools

- **Docker Compose** - Local development environment
- **MailHog** - Email testing and interception
- **Swagger/OpenAPI** - API documentation

## 🌐 Service URLs

When running locally with Docker Compose:

| Service             | URL                           | Purpose                  |
| ------------------- | ----------------------------- | ------------------------ |
| API                 | http://localhost:5100         | Main application API     |
| Swagger UI          | http://localhost:5100/swagger | API documentation        |
| PgAdmin             | http://localhost:5050         | Database management      |
| RabbitMQ Management | http://localhost:16672        | Message queue management |
| Jaeger UI           | http://localhost:16686        | Distributed tracing      |
| MailHog UI          | http://localhost:8025         | Email testing            |

### Default Credentials

- **PgAdmin**: admin@example.com / admin
- **RabbitMQ**: app / app
- **PostgreSQL**: app / app (database: enterprise)

## 🔧 Configuration

### Environment Switching

Configure infrastructure implementation via `appsettings.json`:

```json
{
  "Cloud": "OnPrem", // or "Azure"
  "Auth": {
    "Mode": "None" // or "Jwt"
  }
}
```

### Database Configuration

The application supports multiple database providers:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=enterprise;Username=app;Password=app"
  },
  "Db": {
    "Provider": "Postgres" // or "SqlServer"
  }
}
```

## 🧪 Testing & Validation

### Load Testing

Generate traffic for OpenTelemetry validation:

```powershell
# Simple load test
.\load-test.ps1

# Extended load test with multiple endpoints
.\simple-load-test.ps1
```

### Health Checks

Monitor application health:

- **Liveness**: `GET /health/live`
- **Readiness**: `GET /health/ready`

### Message Testing

Test event publishing:

```bash
curl -X POST http://localhost:5100/bus/ping
```

## 🔍 Observability

### Distributed Tracing

- View traces in Jaeger UI: http://localhost:16686
- Automatic instrumentation for HTTP, Entity Framework, and custom operations
- Correlation IDs for request tracking

### Structured Logging

- JSON-formatted logs with Serilog
- Request/response logging with correlation
- Environment-specific log levels

### Metrics & Monitoring

- Runtime metrics (GC, memory, threads)
- HTTP request metrics (duration, status codes)
- Custom business metrics via OpenTelemetry

## 🏛️ Clean Architecture Benefits

### Separation of Concerns

- **Domain**: Pure business logic, no external dependencies
- **Application**: Use cases, validation, authorization
- **Infrastructure**: External service adapters, database access
- **API**: HTTP concerns, authentication, serialization

### Testability

- Domain logic can be tested in isolation
- Infrastructure dependencies are mockable
- Integration tests with Testcontainers support

### Cloud Portability

- Switch between OnPrem and Azure implementations
- Infrastructure concerns abstracted behind interfaces
- Configuration-driven environment adaptation

## 🚀 Production Deployment

### Docker Support

The application is containerization-ready:

- Multi-stage Dockerfile for optimized images
- Environment-specific configuration
- Health check endpoints for orchestrators

### Cloud Migration

Switch to Azure services by changing configuration:

- Azure SQL Database or PostgreSQL Flexible Server
- Azure Service Bus for messaging
- Azure Key Vault for secrets
- Azure Cache for Redis

### Monitoring

Production observability includes:

- Application Performance Monitoring (APM)
- Distributed tracing across services
- Structured logging for troubleshooting
- Health checks for availability monitoring

## 📚 Additional Documentation

- **Architecture**: `docs/ARCHITECTURE.md` - Detailed system design
- **Environments**: `docs/ENVIRONMENTS.md` - Deployment configurations
- **References**: `docs/REFERENCES.md` - Technology references and patterns

---

Built with ❤️ using Clean Architecture principles and modern .NET practices.
