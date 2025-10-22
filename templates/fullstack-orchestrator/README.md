# EnterpriseFullstack - Fullstack Enterprise Solution

A complete enterprise fullstack solution orchestrating Blazor Server Web application and Clean Architecture API with perfect integration.

## 🚀 Quick Start

This template generates a workspace that coordinates both Web and API templates:

### 1. Generate the Fullstack Workspace

```bash
dotnet new enterprise-fullstack -n MyCompany.Fullstack
cd MyCompany.Fullstack
```

### 2. Start Development Environment

The environment setup will automatically generate the Web and API projects on first run:

```bash
# This will automatically:
# - Generate Web project (if not exists)
# - Generate API project (if not exists)
# - Update .env files with project names
# - Start the full stack
make up-fullstack
```

Or manually generate projects first:

```bash
# Generate projects with custom options
./setup.ps1 -WebProjectName "MyWeb" -APIProjectName "MyApi"

# Then start the environment
make up-fullstack
```

### 3. Access Services

- **Web Application**: http://localhost:3000
- **API**: http://localhost:5100
- **API Documentation**: http://localhost:5100/swagger
- **Database Admin**: http://localhost:5050 (pgAdmin)
- **Message Queue**: http://localhost:16672 (RabbitMQ)
- **Tracing**: http://localhost:16686 (Jaeger)
- **Logging**: http://localhost:5341 (Seq)

## 🏗️ Architecture

### Orchestrated Components

- **Web Template**: Enterprise Blazor Server with backend integration
- **API Template**: Clean Architecture with CQRS/MediatR
- **Shared Infrastructure**: PostgreSQL, Redis, RabbitMQ, Jaeger, Seq

### Perfect Integration

- **Service Sharing**: Both templates use identical infrastructure services
- **Port Coordination**: Web (3000-3002) and API (5000-5002) avoid conflicts
- **Environment Alignment**: Consistent environment variables and service names
- **Development Parity**: Local development mirrors production architecture

## 🔧 Configuration

### Template Parameters Used

- **Cloud**: CLOUD_PROVIDER
- **Database**: DATABASE_PROVIDER
- **Message Bus**: MESSAGE_BUS
- **Caching**: Enabled/Disabled based on selection
- **Observability**: Enabled/Disabled based on selection
- **Logging**: LOGGING_PROVIDER

### Environment Management

```bash
# Development (default)
make up-dev              # Both Web and API in development mode

# Staging
make up-staging          # Both Web and API in staging mode

# Production simulation
make up-prod             # Both Web and API in production mode
```

## 📁 Generated Structure

After running the orchestrator template, you'll have:

```
EnterpriseFullstack/
├── WEB_PROJECT_NAME/           # Blazor Server Web (generated separately)
├── API_PROJECT_NAME/          # Clean Architecture API (generated separately)
├── docker-compose.fullstack.yml  # Orchestrated infrastructure
├── Makefile                   # Unified development commands
├── .env.dev                   # Development environment variables
├── .env.staging               # Staging environment variables
├── .env.prod                 # Production environment variables
└── README.md                 # This file
```

## 🛠️ Development Commands

### Infrastructure Management

```bash
make up-fullstack        # Start Web + API + Infrastructure
make up-web-only          # Start Web Template only
make up-backend-only     # Start API template only
make logs               # View all service logs
make logs-web            # View Web logs only
make logs-backend       # View API logs only
make down              # Stop all services
```

### Environment Switching

```bash
make up ENV=dev         # Development environment
make up ENV=staging     # Staging environment
make up ENV=prod        # Production environment
```

### Health Monitoring

```bash
make health             # Check all service health
make health-web          # Check Web health
make health-backend     # Check API health
```

## 🔍 Service Integration

### Database Sharing

Both Web and API connect to the same PostgreSQL instance:

- **Development**: `enterprisefullstack_dev`
- **Staging**: `enterprisefullstack_staging`
- **Production**: `enterprisefullstack_prod`

### Observability Integration

- **Distributed Tracing**: Both services send traces to shared Jaeger
- **Centralized Logging**: Both services log to shared Seq instance
- **Correlation IDs**: Request correlation across Web and API calls

### Message Bus Coordination

When enabled, both Web and API can:

- Publish messages to shared RabbitMQ instance
- Subscribe to shared message topics
- Coordinate background processing

## 🚀 Production Deployment

### Container Strategy

Each template generates its own Dockerfile:

- Web Template produces optimized Blazor container
- API template produces minimal .NET runtime container
- Infrastructure services use official images

### Environment Promotion

```bash
# Build for production
docker build -t enterprisefullstack-web:latest ./WEB_PROJECT_NAME
docker build -t enterprisefullstack-api:latest ./API_PROJECT_NAME

# Deploy to staging
make deploy ENV=staging

# Deploy to production (after validation)
make deploy ENV=prod
```

## 📚 Documentation

- **Web Template**: See `WEB_PROJECT_NAME/README.md`
- **API Template**: See `API_PROJECT_NAME/README.md`
- **Architecture**: Orchestration patterns and service coordination
- **Deployment**: Production deployment strategies

---

Built with ❤️ for enterprise fullstack development workflows.
