# Backend Template Patterns Analysis

This document captures the exact patterns from the backend template that the web template must align with for perfect integration.

## 🏗️ Infrastructure Architecture

### **Clean Architecture Layers**

```
├── src/Api/                    # ASP.NET Core Web API (presentation layer)
├── src/Application/            # CQRS with MediatR, validation, authorization behaviors
├── src/Domain/                 # Pure business logic, entities, value objects
├── src/Infrastructure/         # Data access, messaging, external service adapters
│   ├── OnPremPack/            # On-premises infrastructure implementations
│   ├── AzurePack/             # Azure cloud infrastructure implementations
│   └── Abstractions/          # ISecrets, IBlobStorage, IEventBus interfaces
└── src/Worker/                # Background service processing
```

### **Switchable Infrastructure Packs**

```csharp
// Program.cs - Configuration-driven switching
var cloud = builder.Configuration["Cloud"] ?? "OnPrem";
if (string.Equals(cloud, "Azure", StringComparison.OrdinalIgnoreCase))
{
    EnterpriseTemplate.Infrastructure.AzurePack.ServiceCollectionExtensions.AddAzurePack(builder.Services, builder.Configuration);
}
else
{
    EnterpriseTemplate.Infrastructure.OnPremPack.ServiceCollectionExtensions.AddOnPremPack(builder.Services, builder.Configuration);
}
```

## 🐳 Docker Architecture

### **Single Compose File with Profiles**

- **File**: `docker-compose.envs.yml` (not multiple files)
- **Profiles**: ["dev", "staging", "prod"]
- **Environment Files**: `.env.dev`, `.env.staging`, `.env.prod`

### **COMPOSE_PROFILES Usage Pattern**

```bash
# Backend Makefile pattern for environment switching
$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f docker-compose.envs.yml up -d
$$env:COMPOSE_PROFILES='staging'; docker compose --env-file .env.staging -f docker-compose.envs.yml up -d
$$env:COMPOSE_PROFILES='prod'; docker compose --env-file .env.prod -f docker-compose.envs.yml up -d
```

## 🛠️ Makefile Architecture

### **PowerShell Detection Pattern**

```makefile
# --- Windows (PowerShell) vs Unix (sh) detection ---
ifeq ($(OS),Windows_NT)
SHELL := pwsh.exe
.SHELLFLAGS := -NoProfile -Command
COMPOSE_FILE := docker-compose.envs.yml

check-compose:
	@if (-not (Get-Command docker -ErrorAction SilentlyContinue)) { Write-Error '❌ Docker CLI not found.'; exit 1 }; if (-not ((docker compose version) 2>$$null)) { Write-Error '❌ `docker compose` not available. Enable Compose V2 in Docker Desktop or install the plugin.'; exit 1 }

up-dev: check-compose
	$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

down-dev: check-compose
	$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) down -v --remove-orphans
```

## 🌍 Environment Configuration

### **Variable Naming Convention**

- Database: `enterprisetemplate_dev`, `enterprisetemplate_staging`, `enterprisetemplate_prod`
- Connection: `DB_USER`, `DB_PASS` (not `DATABASE_USER`, `DATABASE_PASSWORD`)
- Services: `postgres`, `redis`, `rabbitmq`, `jaeger`, `seq`, `otel` (consistent naming)

### **Port Allocation Strategy**

| Service             | Development | Staging | Production |
| ------------------- | ----------- | ------- | ---------- |
| API                 | 5000        | 5001    | 5002       |
| PostgreSQL          | 5432        | 5433    | 5434       |
| RabbitMQ AMQP       | 6672        | 6673    | 6674       |
| RabbitMQ Management | 16672       | 16673   | 16674      |
| Redis               | 6379        | 6380    | 6381       |
| Jaeger UI           | 16686       | 16687   | 16688      |
| OTLP gRPC           | 4317        | 4318    | 4319       |
| MailHog UI          | 8025        | 8026    | 8027       |

### **Environment Files Structure**

```bash
# .env.dev
DB_USER=app
DB_PASS=app
POSTGRES_DB=enterprisetemplate_dev
ASPNETCORE_ENVIRONMENT=Development
OTLP_ENDPOINT=http://localhost:4317

# .env.staging
DB_USER=app
DB_PASS=app_staging
POSTGRES_DB=enterprisetemplate_staging
ASPNETCORE_ENVIRONMENT=Staging
OTLP_ENDPOINT=http://localhost:4318

# .env.prod
DB_USER=app
DB_PASS=app_prod
POSTGRES_DB=enterprisetemplate_prod
ASPNETCORE_ENVIRONMENT=Production
OTLP_ENDPOINT=http://localhost:4319
```

## 🔍 Observability Stack

### **OpenTelemetry Configuration**

```csharp
// Program.cs - Minimal OpenTelemetry setup
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter());
```

### **Structured Logging with Serilog**

```csharp
// Program.cs - Serilog configuration
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
```

### **Health Check Endpoints**

- `/health/live` - Liveness probe (application responsive)
- `/health/ready` - Readiness probe (dependencies healthy)

## 🏛️ Technology Stack

### **Core Framework**

- **.NET 9.0** / ASP.NET Core 9.0
- **Entity Framework Core 9.0** with PostgreSQL/SQL Server
- **MediatR 12.4.1** for CQRS implementation
- **FluentValidation 11.10.0** for input validation

### **Infrastructure Services**

- **PostgreSQL 16** - Primary database with health checks
- **RabbitMQ 3** - Message broker with MassTransit integration
- **Redis 7** - Distributed caching layer
- **Jaeger** - Distributed tracing and observability
- **Seq** - Centralized structured logging
- **MailHog** - Email testing and interception

### **Cross-Cutting Concerns**

- **Authentication**: JWT Bearer with configurable authority or No-Auth mode
- **Authorization**: Policy-based with `Todos.Read` and `Todos.Create` policies
- **Rate Limiting**: Token bucket algorithm (20 req/sec, 40 burst)
- **Security Headers**: X-Content-Type-Options, Referrer-Policy, X-XSS-Protection, X-Frame-Options
- **Resilience**: Polly patterns (retry, circuit breaker, timeout) via `IResilienceService`
- **Validation**: FluentValidation with MediatR pipeline behaviors

## 📋 Template Parameters

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

## 🎯 Integration Requirements for Web Template

To achieve perfect backend integration, the web template must:

### 1. **Match Makefile Structure**

- Use identical PowerShell detection logic
- Implement COMPOSE_PROFILES environment variable usage
- Use same compose file naming (`docker-compose.envs.yml`)

### 2. **Align Docker Architecture**

- Single compose file with profiles ["dev", "staging", "prod"]
- Environment-specific `.env` files with backend naming
- Service names match backend exactly

### 3. **Use Backend Variable Names**

- `DB_USER`, `DB_PASS` (not `DATABASE_USER`, `DATABASE_PASSWORD`)
- Database names: `enterprisetemplate_dev`, etc.
- Service names: `postgres`, `redis`, `rabbitmq`, `jaeger`, `seq`

### 4. **Port Coordination**

- Web uses different API ports (3000, 3001, 3002) to avoid conflicts
- Infrastructure services share same ports as backend
- Perfect service sharing through identical service names

### 5. **Environment Management**

- Match backend environment file structure
- Use identical ASPNETCORE_ENVIRONMENT values
- Coordinate OTLP_ENDPOINT configurations

This alignment ensures seamless service sharing and perfect integration when both templates run simultaneously.
