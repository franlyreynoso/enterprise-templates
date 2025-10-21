# Enterprise Clean Architecture Template

A comprehensive enterprise-grade .NET solution template featuring Clean Architecture with all cross-cutting concerns implemented.

## 🚀 Features

### Core Architecture

- **Clean Architecture** with Domain, Application, Infrastructure, and API layers
- **CQRS + MediatR** for command/query separation
- **Entity Framework Core** with proper repository patterns
- **Dependency Injection** with proper service lifetimes

### Enterprise Cross-Cutting Concerns

- ✅ **Security & Authorization**: JWT Bearer authentication, HTTPS, security headers
- ✅ **Distributed Caching**: Redis with cache-aside pattern and health checks
- ✅ **Health Checks**: Database, Redis, RabbitMQ, external service monitoring
- ✅ **Logging & Observability**: Serilog + Seq + OpenTelemetry + Jaeger
- ✅ **Rate Limiting**: Token bucket algorithm with global partitioning
- ✅ **Configuration Management**: Environment-specific settings with cloud adapters
- ✅ **Background Processing**: MassTransit with configurable transports
- ✅ **Data Validation**: FluentValidation with MediatR pipeline
- ✅ **Resilience Patterns**: Polly retry policies, circuit breakers, timeouts
- ✅ **API Documentation**: Swagger/OpenAPI integration

### Infrastructure Options

- **Database**: PostgreSQL or SQL Server with EF Core migrations
- **Caching**: Redis distributed cache (optional)
- **Messaging**: RabbitMQ, Azure Service Bus, or None
- **Monitoring**: Comprehensive health checks and observability
- **Logging**: Seq, ElasticSearch, or Console
- **Cloud**: Azure or OnPrem deployment configurations

## 📋 Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose
- PowerShell (for Makefile on Windows)

## 🛠️ Usage

### Install Template

```bash
dotnet new install path/to/template
```

### Create New Project

```bash
# Basic usage with defaults
dotnet new enterprise-clean -n MyCompany.MyProject

# Full customization
dotnet new enterprise-clean -n MyCompany.MyProject \
  --cloud "Azure" \
  --db "SqlServer" \
  --bus "AzureServiceBus" \
  --multitenant true \
  --caching true \
  --monitoring true \
  --resilience true \
  --logging "ElasticSearch"
```

### Start Development Environment

```bash
# Start all services
make up-dev

# Start the API
dotnet run --project src/Api
```

## 🔧 Template Parameters

| Parameter     | Type   | Default    | Description                                  |
| ------------- | ------ | ---------- | -------------------------------------------- |
| `cloud`       | choice | "OnPrem"   | Target cloud adapter (OnPrem/Azure)          |
| `db`          | choice | "Postgres" | Database provider (Postgres/SqlServer)       |
| `bus`         | choice | "RabbitMQ" | Message bus (RabbitMQ/AzureServiceBus/None)  |
| `multitenant` | bool   | false      | Enable multitenancy scaffolding              |
| `caching`     | bool   | true       | Include Redis distributed caching            |
| `monitoring`  | bool   | true       | Include health checks and monitoring         |
| `resilience`  | bool   | true       | Include circuit breakers and resilience      |
| `logging`     | choice | "Seq"      | Logging provider (Seq/ElasticSearch/Console) |

## 📚 Documentation

- **Architecture**: See `docs/ARCHITECTURE.md`
- **Environments**: See `docs/ENVIRONMENTS.md`
- **References**: See `docs/REFERENCES.md`

## 🚀 Quick Start

1. **Clone and customize** the template
2. **Start infrastructure**: `make up-dev`
3. **Run the API**: `dotnet run --project src/Api`
4. **Access services**:
   - API: http://localhost:5000
   - Swagger: http://localhost:5000/swagger
   - Seq Logs: http://localhost:5341
   - Jaeger Tracing: http://localhost:16686
   - RabbitMQ: http://localhost:15672

## 📝 License

MIT License - feel free to use this template for your projects.
