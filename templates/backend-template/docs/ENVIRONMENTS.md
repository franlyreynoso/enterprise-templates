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

**API Startup**:

```bash
dotnet run --project src/Api -- --urls http://localhost:5100 --environment Development
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

### **Environment Variables**

Each environment uses dedicated `.env` files:

| Setting       | Development              | Staging                      | Production                |
| ------------- | ------------------------ | ---------------------------- | ------------------------- |
| Database      | `EnterpriseTemplate_dev` | `EnterpriseTemplate_staging` | `EnterpriseTemplate_prod` |
| Auth Mode     | `None`                   | `Jwt`                        | `Jwt`                     |
| Log Level     | `Debug`                  | `Information`                | `Warning`                 |
| OTLP Endpoint | `localhost:4317`         | `staging-jaeger:4317`        | `prod-jaeger:4317`        |

### **Connection Strings**

Environment-specific database connections:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database={env}_database;Username=app;Password=app"
  }
}
```

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

---

This environment strategy ensures consistency, reliability, and smooth progression from development through production deployment.
