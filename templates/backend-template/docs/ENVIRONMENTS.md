# Environment Strategy

The EnterpriseTemplate microservice supports multiple deployment environments (**Development**, **Staging**, and **Pro| Database | `enterprisetemplate_dev` | `enterprisetemplate_staging` | `enterprisetemplate_prod` |uction**) with consistent infrastructure stacks. Each environment uses the same technology components (PostgreSQL, RabbitMQ, Redis, Jaeger, MailHog) but with environment-specific configurations for isolation and testing.

## 🎯 Why This Approach?

### **Development-Production Parity**

- **Consistency**: Local development closely mirrors higher environments
- **Predictability**: Reduces "works on my machine" issues
- **Testing**: Integration tests run against the same stack as production

### **Simplified Operations**

- **Single Docker Compose**: One file with environment profiles
- **Environment Variables**: Configuration through `.env` files
- **Makefile Automation**: Consistent commands across environments

### **Future Portability**

- **Container-Ready**: Easy migration to Kubernetes or cloud platforms
- **Infrastructure as Code**: Reproducible environment definitions
- **Cloud Agnostic**: Switch between cloud providers without code changes

## 🏗️ Environment Architecture

### **Infrastructure Stack**

Each environment includes:

- **PostgreSQL 16** - Primary database with health checks
- **RabbitMQ 3** - Message broker with management UI
- **Redis 7** - Distributed caching layer
- **Jaeger** - Distributed tracing and observability
- **MailHog** - Email testing and interception

### **Environment Isolation**

- **Database**: Separate databases per environment (`enterprisetemplate_dev`, `enterprisetemplate_staging`, `enterprisetemplate_prod`)
- **Ports**: Configurable port mappings to avoid conflicts
- **Credentials**: Environment-specific authentication
- **Resource Limits**: Scaled appropriately per environment

## 🚀 Environment Management

### **Development Environment**

**Purpose**: Local development with debug-friendly settings

```bash
# Start development stack
make up-dev

# Or manually
docker compose --env-file .env.dev -f docker-compose.envs.yml up -d --profile dev
```

**Configuration**:

- Database: `enterprisetemplate_dev` with relaxed settings
- Authentication: Disabled for easy testing (`Auth.Mode: "None"`)
- Logging: Debug level for detailed troubleshooting
- RabbitMQ: Custom ports (6672/16672) to avoid conflicts
- Health Checks: Fast intervals for quick feedback

**API Startup Options**:

**Option 1: Containerized (Recommended)**
```bash
# API runs in Docker container with ASPNETCORE_ENVIRONMENT=Development
# Uses appsettings.Development.json with Docker service names
docker compose up api
```

**Option 2: Local Machine**
```bash
# API runs directly on host machine, connecting to containerized services
# Uses appsettings.Local.json with localhost connections
$env:ASPNETCORE_ENVIRONMENT="Local"
dotnet run --project src/Api -- --urls http://localhost:5100
```

### **Staging Environment**

**Purpose**: Pre-production testing with production-like settings

```bash
# Start staging stack
make up-staging

# Or manually
docker compose --env-file .env.staging -f docker-compose.envs.yml up -d --profile staging
```

**Configuration**:

- Database: `enterprisetemplate_staging` with production settings
- Authentication: JWT mode with staging identity provider
- Logging: Information level for operational insights
- Resource Limits: Production-equivalent sizing
- Health Checks: Production intervals

**API Startup**:

```bash
$env:ASPNETCORE_ENVIRONMENT="Staging"
dotnet run --project src/Api -- --urls http://localhost:5200 --environment Staging
```

### **Production Environment** (Local Simulation)

**Purpose**: Local production simulation for deployment validation

```bash
# Start production stack
make up-prod

# Or manually
docker compose --env-file .env.prod -f docker-compose.envs.yml up -d --profile prod
```

**Configuration**:

- Database: `enterprisetemplate_prod` with production-grade settings
- Authentication: Full JWT validation with production authority
- Logging: Warning level for performance
- Security: All security features enabled
- Health Checks: Production-optimized intervals

**API Startup**:

```bash
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run --project src/Api -- --urls http://localhost:5300 --environment Production
```

## ⚙️ Configuration Strategy

### **Hierarchical Configuration**

ASP.NET Core configuration layering:

1. `appsettings.json` - Base settings
2. `appsettings.{Environment}.json` - Environment-specific overrides
3. Environment variables - Runtime overrides
4. Command-line arguments - Final overrides

### **Appsettings File Usage**

The template includes multiple appsettings files for different scenarios:

| File | Environment | When to Use | Connection Targets |
|------|-------------|-------------|-------------------|
| `appsettings.json` | All | Base configuration shared across all environments | N/A |
| `appsettings.Development.json` | Development | **Containerized development** (API running in Docker) | Docker service names (`postgres`, `redis`, `rabbitmq`, `seq`, `otel`) |
| `appsettings.Local.json` | Local | **Local machine development** (API running outside containers via `dotnet run`) | Localhost services (`localhost:5432`, `localhost:6379`, etc.) |
| `appsettings.Staging.json` | Staging | Pre-production environment | Environment-specific endpoints |
| `appsettings.Production.json` | Production | Production environment | Production endpoints |

**Important Notes:**
- **Development Environment (ASPNETCORE_ENVIRONMENT=Development)**: Used when running the API in a Docker container. Uses Docker service names for networking between containers.
- **Local Environment (ASPNETCORE_ENVIRONMENT=Local)**: Used when running the API directly on your machine with `dotnet run`. Connects to infrastructure services via localhost ports.

To run in Local environment:
```bash
# Set the environment variable
$env:ASPNETCORE_ENVIRONMENT="Local"  # Windows PowerShell
export ASPNETCORE_ENVIRONMENT=Local   # Linux/macOS

# Run the API
dotnet run --project src/Api
```

### **Environment Variables**

Each environment uses dedicated `.env` files:

| Setting       | Development              | Staging                      | Production                |
| ------------- | ------------------------ | ---------------------------- | ------------------------- |
| Database      | `EnterpriseTemplate_dev` | `EnterpriseTemplate_staging` | `EnterpriseTemplate_prod` |
| Auth Mode     | `None`                   | `Jwt`                        | `Jwt`                     |
| Log Level     | `Debug`                  | `Information`                | `Warning`                 |
| OTLP Endpoint | `localhost:4317`         | `staging-jaeger:4317`        | `prod-jaeger:4317`        |

### **Connection Strings**

Connection strings are environment-specific based on where the API is running:

**Development (Containerized API)**
```json
{
  "ConnectionStrings": {
    "Default": "Host=postgres;Port=5432;Database=enterprisetemplate_dev;Username=app;Password=app",
    "Redis": "redis:6379"
  },
  "RabbitMQ": {
    "Host": "rabbitmq",
    "Port": 5672
  },
  "Telemetry": { "OtlpExporterUrl": "http://otel:4317" },
  "Serilog": {
    "WriteTo": [{ "Name": "Seq", "Args": { "serverUrl": "http://seq:80" } }]
  }
}
```

**Local (API on Host Machine)**
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=enterprisetemplate_dev;Username=app;Password=app",
    "Redis": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 6672
  },
  "Telemetry": { "OtlpExporterUrl": "http://localhost:4317" },
  "Serilog": {
    "WriteTo": [{ "Name": "Seq", "Args": { "serverUrl": "http://localhost:5341" } }]
  }
}
```

**Why the Difference?**
- **Docker networking**: Containers communicate using service names defined in `docker-compose.yml`
- **Host networking**: Applications on the host machine connect via `localhost` and mapped ports

### **Cloud Switching**

Infrastructure implementation selection:

```json
{
  "Cloud": "OnPrem", // Development & Staging
  "Cloud": "Azure" // Production (optional)
}
```

## 🔧 Service Configuration

### **Port Mappings**

Consistent port allocation across environments:

| Service             | Development | Staging | Production |
| ------------------- | ----------- | ------- | ---------- |
| API                 | 5100        | 5200    | 5300       |
| PostgreSQL          | 5432        | 5433    | 5434       |
| RabbitMQ AMQP       | 6672        | 6673    | 6674       |
| RabbitMQ Management | 16672       | 16673   | 16674      |
| Redis               | 6379        | 6380    | 6381       |
| Jaeger UI           | 16686       | 16687   | 16688      |
| OTLP gRPC           | 4317        | 4318    | 4319       |
| MailHog UI          | 8025        | 8026    | 8027       |

### **Health Checks**

Environment-tuned health check settings:

- **Development**: Fast intervals (5s) for quick feedback
- **Staging**: Balanced intervals (10s) for stability testing
- **Production**: Conservative intervals (30s) for reliability

### **Resource Limits**

Environment-appropriate resource allocation:

- **Development**: Minimal resources for laptop development
- **Staging**: Production-equivalent for realistic testing
- **Production**: Full production specifications

## 🏃‍♂️ Quick Commands

### **Environment Management**

```bash
# Start environments
make up-dev      # Development environment
make up-staging  # Staging environment
make up-prod     # Production environment

# Stop environments
make down-dev    # Stop and clean development
make down-staging # Stop and clean staging
make down-prod   # Stop and clean production
```

### **API Testing**

```bash
# Development
curl http://localhost:5100/health/live

# Staging
curl http://localhost:5200/health/live

# Production
curl http://localhost:5300/health/live
```

### **Service Access**

Development environment URLs:

- API: http://localhost:5100
- Swagger: http://localhost:5100/swagger
- RabbitMQ: http://localhost:16672 (app/app)
- Jaeger: http://localhost:16686
- MailHog: http://localhost:8025

## 🚀 CI/CD Integration

### **Pipeline Strategy**

```yaml
# Conceptual pipeline flow
stages:
  - build: Compile and package application
  - test: Unit tests + integration tests with Testcontainers
  - staging-deploy: Deploy to staging environment
  - staging-test: End-to-end testing in staging
  - production-deploy: Manual approval → production deployment
```

### **Container Registry**

```bash
# Build production image
docker build -t enterprisetemplate:latest .

# Tag for environment
docker tag enterprisetemplate:latest registry.com/enterprisetemplate:staging
docker tag enterprisetemplate:latest registry.com/enterprisetemplate:prod
```

### **Deployment Strategies**

- **Blue-Green**: Zero-downtime deployments with instant rollback
- **Canary**: Gradual traffic shifting for risk mitigation
- **Rolling**: Sequential instance updates with health validation

## 🔍 Environment Monitoring

### **Health Endpoints**

Each environment exposes standardized health checks:

- `/health/live` - Liveness probe (application responsive)
- `/health/ready` - Readiness probe (dependencies healthy)

### **Observability Stack**

- **Metrics**: OpenTelemetry metrics exported to monitoring systems
- **Tracing**: Distributed traces in Jaeger for request flow analysis
- **Logging**: Structured logs with correlation IDs
- **Alerting**: Environment-specific alert thresholds

### **Performance Monitoring**

- **Development**: Focus on development-time metrics
- **Staging**: Load testing and performance validation
- **Production**: SLA monitoring and business metrics

## 📋 Environment Checklist

### **Before Deployment**

- [ ] Environment variables configured
- [ ] Database migrations tested
- [ ] Health checks responding
- [ ] Authentication/authorization working
- [ ] Message queue connectivity verified
- [ ] Tracing and logging operational

### **Post-Deployment Validation**

- [ ] Application startup successful
- [ ] All health endpoints green
- [ ] Sample API requests working
- [ ] Message publishing/consuming functional
- [ ] Database queries executing
- [ ] Traces appearing in Jaeger

## 🐛 Troubleshooting

### **Connection Issues**

**Problem**: API cannot connect to database or other services

**Solution**: Verify you're using the correct appsettings file for your environment

1. **Running API in Docker container**:
   - Ensure `ASPNETCORE_ENVIRONMENT=Development`
   - Uses `appsettings.Development.json` with Docker service names
   - Check: `docker logs <container-name>` for connection errors

2. **Running API on host machine**:
   - Set `ASPNETCORE_ENVIRONMENT=Local`
   - Uses `appsettings.Local.json` with localhost connections
   - Ensure infrastructure services are running: `make up-dev`

3. **Verify connectivity**:
   ```bash
   # From host machine
   psql -h localhost -p 5432 -U app -d enterprisetemplate_dev
   
   # From inside API container
   docker exec -it <api-container> /bin/bash
   apt-get update && apt-get install -y postgresql-client
   psql -h postgres -p 5432 -U app -d enterprisetemplate_dev
   ```

### **Common Mistakes**

| Issue | Cause | Fix |
|-------|-------|-----|
| "Connection refused" when API in container | Using `localhost` instead of service names | Set `ASPNETCORE_ENVIRONMENT=Development` (not Local) |
| "Connection refused" when running `dotnet run` | Using service names instead of localhost | Set `ASPNETCORE_ENVIRONMENT=Local` |
| Wrong database | Using incorrect appsettings file | Check environment variable and appsettings file content |

---

This environment strategy ensures consistency, reliability, and smooth progression from development through production deployment.
