# EnterpriseTemplate Architecture

## Overview

This is a .NET 9.0 microservice implementing Clean Architecture principles with CQRS, messaging, and comprehensive observability. The system is designed for cloud portability with switchable infrastructure implementations.

## Layers

### **Api Layer (Presentation)**

- **Framework**: ASP.NET Core 9.0 Web API
- **Authentication**: Configurable JWT Bearer or No-Auth mode
- **Authorization**: Policy-based authorization with custom behaviors
- **API Documentation**: OpenAPI/Swagger with Swashbuckle
- **Security Headers**: NWebsec middleware (XSS, CSRF, Content-Type protection)
- **Rate Limiting**: Token bucket algorithm (20 req/sec, 40 burst)
- **Health Checks**: `/health/live` and `/health/ready` endpoints
- **Global Exception Handling**: RFC7807 ProblemDetails with structured error responses

### **Application Layer (Business Logic)**

- **CQRS**: MediatR with command/query separation
- **Validation**: FluentValidation with pipeline behaviors
- **Authorization**: Custom `AuthorizationBehavior` for business rules
- **Current Implementation**: Todo management (Create, List) with user context
- **Messaging**: Event publishing via MassTransit abstractions

### **Domain Layer (Business Rules)**

- **Principles**: Pure business logic, framework-agnostic
- **Value Objects**: Shared primitives and domain concepts
- **Current State**: Minimal implementation ready for business rule expansion

### **Infrastructure Layer (Adapters)**

- **Pattern**: Ports & Adapters with switchable implementations
- **Database**: Entity Framework Core 9.0 with PostgreSQL/SQL Server support
- **Messaging**: MassTransit 8.2.4 with RabbitMQ
- **Caching**: Redis via StackExchange.Redis
- **Secrets**: Environment variables (OnPrem) or Azure Key Vault (Azure)
- **Storage**: File system (OnPrem) or Azure Blob Storage (Azure)

## Technology Stack

### **Core Framework**

- .NET 9.0 / ASP.NET Core 9.0
- Entity Framework Core 9.0.0
- MediatR 12.4.1 for CQRS
- FluentValidation 11.10.0

### **Data Persistence**

- **Primary Database**: PostgreSQL 16 with Npgsql provider
- **Alternative**: SQL Server support via EF Core
- **Database Management**: PgAdmin 4 web interface
- **Migrations**: EF Core Code-First with automatic migration on startup

### **Messaging & Integration**

- **Message Bus**: MassTransit 8.2.4 with RabbitMQ 3-management
- **Queue Management**: RabbitMQ Management UI
- **Event Publishing**: Async messaging patterns

### **Observability & Monitoring**

- **Logging**: Serilog with structured logging and request correlation
- **Tracing**: OpenTelemetry 1.9.0 with OTLP export to Jaeger
- **Metrics**: ASP.NET Core and runtime instrumentation
- **APM**: Jaeger UI for distributed tracing analysis

### **Development Infrastructure**

- **Containerization**: Docker Compose for local development
- **Package Management**: Centralized with Directory.Packages.props
- **Email Testing**: MailHog for SMTP interception
- **Configuration**: Environment-based with appsettings.json hierarchy

## Cloud Portability

### **Configuration-Driven Switching**

The `Cloud` configuration setting switches between infrastructure implementations:

- `Cloud=OnPrem` → Uses `AddOnPremPack()`
- `Cloud=Azure` → Uses `AddAzurePack()`

### **OnPrem Pack (Default)**

- **Database**: PostgreSQL with direct connection strings
- **Messaging**: RabbitMQ with AMQP protocol
- **Secrets**: Environment variables and file-based configuration
- **Storage**: Local file system
- **Caching**: Redis standalone

### **Azure Pack (Cloud-Ready)**

- **Database**: Azure SQL Database or PostgreSQL Flexible Server
- **Messaging**: Azure Service Bus with MassTransit integration
- **Secrets**: Azure Key Vault integration
- **Storage**: Azure Blob Storage
- **Caching**: Azure Cache for Redis

## Security Architecture

### **Authentication Modes**

- **JWT Mode**: OIDC/JWT Bearer token validation with configurable authority
- **Development Mode**: No authentication for local development
- **Authorization**: Policy-based with `Todos.Read` and `Todos.Create` policies

### **Security Measures**

- **HTTPS Enforcement**: Configurable based on environment
- **Security Headers**: X-Content-Type-Options, Referrer-Policy, X-XSS-Protection, X-Frame-Options
- **Business Authorization**: Application-layer authorization behavior without HttpContext leakage
- **Validation**: Input validation at API boundaries with FluentValidation

## Development Environment

### **Local Infrastructure (Docker Compose)**

- **PostgreSQL**: Database server on port 5432
- **PgAdmin**: Database management UI on port 5050
- **RabbitMQ**: Message broker with management UI on ports 6672/16672
- **Redis**: Caching server on port 6379
- **Jaeger**: Distributed tracing UI on port 16686
- **MailHog**: Email testing on ports 1025/8025

### **API Endpoints**

- **Health**: `/health/live`, `/health/ready`
- **Todos**: `GET /todos`, `POST /todos`
- **Messaging**: `POST /bus/ping` (test message publishing)
- **Testing**: `GET /test/external` (HTTP client tracing)
- **Documentation**: `/swagger` (development only)

## Data Architecture

### **Entity Framework Context**

- **AppDbContext**: Primary database context with PostgreSQL/SQL Server support
- **Migrations**: Automatic execution on application startup
- **Connection Management**: Configuration-driven connection strings
- **Todo Entity**: Current domain model with user association

### **Repository Pattern**

- **ITodoStore**: Abstraction for todo data operations
- **InMemoryTodoStore**: In-memory implementation for testing
- **EfTodoStore**: Entity Framework implementation for persistence

## Observability Strategy

### **Distributed Tracing**

- **OpenTelemetry**: Automatic instrumentation for ASP.NET Core, HTTP clients, and Entity Framework
- **Jaeger Export**: OTLP protocol for trace aggregation and analysis
- **Correlation**: Request correlation IDs throughout the call chain

### **Structured Logging**

- **Serilog**: Structured logging with JSON output
- **Request Logging**: HTTP request/response logging with Serilog middleware
- **Configuration**: Environment-specific log levels and sinks

### **Metrics & Monitoring**

- **Runtime Metrics**: GC, memory, and thread pool instrumentation
- **HTTP Metrics**: Request duration, status codes, and throughput
- **Custom Metrics**: Business-specific metrics via OpenTelemetry

## Development Workflow

### **Build & Package Management**

- **Centralized Packages**: Directory.Packages.props with version management
- **Multi-Project Solution**: Clean separation of concerns across layers
- **Build Configuration**: Directory.Build.props for common properties

### **Testing Strategy**

- **Unit Testing**: xUnit framework with test containers for integration tests
- **Load Testing**: PowerShell scripts for OpenTelemetry validation
- **Health Monitoring**: Built-in health check endpoints

### **Deployment Readiness**

- **Configuration**: Environment-specific appsettings files
- **Docker Support**: Containerization ready for cloud deployment
- **Migration Strategy**: Automatic database migrations on startup
- **Graceful Shutdown**: Proper resource cleanup and connection management
