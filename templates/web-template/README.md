# Enterprise UI Template (Full)

A comprehensive enterprise-ready Blazor Server template with modern web development patterns, security, observability, and testing infrastructure.

## 🎯 Features

### ✅ **Authentication & Authorization**

- **OIDC Integration**: OpenID Connect authentication with configurable identity providers
- **Role-Based Access Control**: Built-in authorization patterns with role checking
- **Enterprise Auth Extensions**: Custom authentication helpers and middleware

### ✅ **HTTP & Resilience**

- **Typed HTTP Clients**: Strongly-typed API clients with dependency injection
- **Polly Integration**: Retry policies, circuit breakers, and timeout handling
- **Authentication Forwarding**: Seamless token propagation to downstream services
- **Correlation IDs**: Request tracking across service boundaries

### ✅ **API Code Generation**

- **NSwag Integration**: Automatic TypeScript/C# client generation from OpenAPI specs
- **Strongly-Typed APIs**: Type-safe API interactions with generated models

### ✅ **Modern UI & Theming**

- **MudBlazor Components**: Rich UI component library with Material Design
- **Dark/Light Themes**: Configurable theming with enterprise color schemes
- **Responsive Layout**: Mobile-first responsive design patterns
- **Component Library**: Reusable enterprise UI components

### ✅ **Error Handling**

- **Error Boundaries**: Blazor error boundary components with graceful degradation
- **Global Exception Handling**: Centralized error processing and logging
- **User-Friendly Error Pages**: Custom error displays with actionable messages

### ✅ **Observability**

- **OpenTelemetry**: Distributed tracing and metrics collection
- **Health Checks**: Application and dependency health monitoring
- **Structured Logging**: JSON-formatted logs with correlation context
- **Performance Monitoring**: Request/response timing and throughput metrics

### ✅ **Internationalization**

- **Localization Support**: Multi-language resource management
- **Culture-Aware Components**: Localized date, number, and currency formatting
- **Resource Injection**: Dependency injection for localized strings

### ✅ **Feature Flags**

- **Configuration-Based**: Toggle features via appsettings.json
- **Runtime Switching**: Dynamic feature enabling without deployments
- **Provider Pattern**: Extensible feature flag providers (configuration, external services)

### ✅ **Security**

- **Content Security Policy**: Comprehensive CSP headers with Google Fonts support
- **Security Headers**: X-Frame-Options, X-Content-Type-Options, Referrer-Policy
- **HTTPS Enforcement**: Automatic HTTPS redirection in production
- **Anti-Forgery Tokens**: CSRF protection for form submissions

### ✅ **Testing Infrastructure**

- **bUnit**: Blazor component unit testing framework
- **Playwright**: End-to-end browser automation testing
- **Test Isolation**: Independent test execution with proper setup/teardown
- **CI Integration**: GitHub Actions workflow for automated testing

### ✅ **CI/CD Pipeline**

- **GitHub Actions**: Automated build, test, and deployment workflows
- **Multi-Environment**: Support for development, staging, and production deployments
- **Docker Support**: Containerization-ready configuration

### ✅ **Configuration Management**

- **Central Package Management**: Centralized NuGet package version management
- **Strongly-Typed Options**: Configuration binding with validation
- **Environment-Specific**: Development, staging, and production configurations
- **Secrets Management**: Azure Key Vault and user secrets integration

## 🏗️ Architecture

### Project Structure

```
src/
├── Enterprise.App/                 # Main Blazor Server application
│   ├── Components/                 # Application-specific components
│   ├── Pages/                      # Blazor pages and routing
│   ├── Services/                   # Application services (HealthService)
│   └── Properties/                 # Launch settings and configurations
├── Enterprise.Ui.ApiClient/        # Generated API clients (NSwag)
├── Enterprise.Ui.Auth/             # Authentication & authorization extensions
├── Enterprise.Ui.FeatureFlags/     # Feature flag provider implementations
├── Enterprise.Ui.Http/             # HTTP client extensions & Polly policies
├── Enterprise.Ui.Observability/    # OpenTelemetry & monitoring setup
├── Enterprise.Ui.Security/         # Security headers & CSP configuration
└── Enterprise.Ui.Shared/           # Shared UI components & layouts
    ├── Components/                 # Reusable Blazor components
    ├── Layouts/                    # Application layouts
    └── Theme/                      # MudBlazor theme configuration

tests/
└── Enterprise.Ui.Tests/            # bUnit & Playwright test suite

docs/                               # Documentation
├── ARCHITECTURE.md                 # Detailed architecture guide
├── SECURITY.md                     # Security implementation details
├── CONFIGURATION.md                # Configuration management guide
├── TESTING.md                      # Testing strategies and patterns
├── OBSERVABILITY.md                # Monitoring and telemetry setup
└── DEVELOPMENT.md                  # Development environment setup
```

### Technology Stack

- **.NET 9.0**: Latest .NET framework with modern C# features
- **Blazor Server**: Server-side rendering with SignalR connectivity
- **MudBlazor 7.8.0**: Material Design component library
- **OpenTelemetry 1.9.0**: Observability and distributed tracing
- **Polly 8.4.2**: Resilience and transient fault handling
- **NSwag 14.1.0**: OpenAPI client code generation
- **bUnit**: Blazor component testing framework
- **Playwright**: Cross-browser end-to-end testing

## 🚀 Quick Start

### Prerequisites

- **.NET 9.0 SDK** or later
- **Visual Studio 2024** or **Visual Studio Code** with C# extension
- **Node.js 18+** (for Playwright tests)

### 1. Clone and Restore

```bash
git clone <repository-url>
cd enterprise-ui-template-full
dotnet restore
```

### 2. Automatic Configuration Management

This template includes an **intelligent configuration system** that eliminates manual setup:

#### When Creating from Template

The template automatically generates clean, feature-specific `appsettings.json` files during instantiation using:

- **Modular configuration fragments** in `src/Enterprise.App/Config/`
- **PowerShell merge script** that combines fragments based on selected features
- **Post-actions** that run automatically after template generation

#### Configuration Fragments

Each enterprise feature has its own configuration fragment:

- `auth.json` - Authentication settings
- `http.json` - HTTP resilience configuration
- `features.json` - Feature flag definitions
- `observability.json` - OpenTelemetry settings
- `security.json` - Security headers and CSP

#### Manual Regeneration

To update configuration after template creation:

```bash
cd src/Enterprise.App
.\merge-config.ps1 -Environment Development -EnableAuth -EnableObservability
```

See `src/Enterprise.App/CONFIG.md` for complete documentation.

### 3. Choose Your Development Workflow

#### 🐳 **Docker Development (Backend Integration Aligned)**

```bash
# Environment-specific commands (matches backend template)
make up ENV=dev         # Start development environment
make up ENV=staging     # Start staging environment
make up ENV=prod        # Start production environment
make logs ENV=dev       # View environment logs
make down ENV=dev       # Stop environment

# Quick shortcuts
make up-dev             # Start development (shortcut)
make up-staging         # Start staging (shortcut)
make up-prod           # Start production (shortcut)

# Full-stack integration (UI + Backend simultaneously)
make up-integrated      # Start integrated environment
```

#### 🌟 **Observability & Monitoring**

```bash
# Access observability services
make logs-seq          # Open Seq centralized logging
make logs-otel         # Open Jaeger distributed tracing

# Direct URLs
# Seq Logs: http://localhost:5341
# Jaeger Tracing: http://localhost:16686
# pgAdmin (dev): http://localhost:5050
```

#### 🏃 **Local Development (No Docker)**

```bash
make restore         # Restore NuGet packages
make run            # Run application locally
```

#### 📱 **VS Code Integration**

- **Command Palette** (Ctrl+Shift+P) → "Tasks: Run Task"
- **Dev Container**: "Dev Containers: Reopen in Container"
- Pre-configured tasks for common operations

### 4. Access Your Application

- **🌐 UI**: https://localhost:7000
- **💚 Health Checks**: https://localhost:7000/health
- **🔧 API** (integrated): https://localhost:8001
- **📊 Jaeger Tracing**: http://localhost:16686
- **📈 Prometheus**: http://localhost:9090
- **📋 Grafana**: http://localhost:3000

### 5. Run Tests

```bash
# All available testing commands
make test            # Run all tests
make test-unit       # Unit tests only
make test-integration # Integration tests only
make test-e2e        # End-to-end tests (installs Playwright automatically)

# Traditional approach
dotnet test          # Unit and integration tests
```

## 🔧 Development

### 🎯 Unified Command System

This template uses **Make commands** for consistent development workflows across UI and backend projects:

| Command               | Description                            |
| --------------------- | -------------------------------------- |
| `make help`           | Show all available commands            |
| `make up ENV=dev`     | Start environment (dev/staging/prod)   |
| `make up-dev`         | Quick start development                |
| `make up-integrated`  | Start full-stack integration           |
| `make logs ENV=dev`   | View environment logs                  |
| `make logs-seq`       | Access Seq centralized logging         |
| `make logs-otel`      | Access Jaeger distributed tracing      |
| `make config ENV=dev` | Generate environment configuration     |
| `make test`           | Run unit tests                         |
| `make test-e2e`       | Run end-to-end tests (auto Playwright) |
| `make clean`          | Clean containers and build artifacts   |
| `make run`            | Run locally (no Docker)                |
| `make logs`           | View container logs                    |
| `make clean`          | Clean Docker environment               |
| `make config-dev`     | Generate development config            |
| `make health`         | Check application health               |

### 🎪 VS Code Integration

**Tasks** (Ctrl+Shift+P → "Tasks: Run Task"):

- 🚀 Start Development Environment
- 🌟 Start Full-Stack Environment
- 🔨 Build Application
- 🧪 Run All Tests
- ❤️ Check Health

**Dev Containers**: Full containerized development environment with all dependencies pre-installed.

### Health Checks

- **Local Health**: `/health` - Application health status
- **Readiness**: `/health/ready` - Dependency readiness checks
- **Liveness**: `/health/live` - Application liveness probe

### Feature Flags

Configure in `appsettings.json`:

```json
{
  "FeatureFlags": {
    "NewNav": "true",
    "AdvancedSearch": "false"
  }
}
```

### Security Headers

Content Security Policy and security headers are automatically applied:

- Google Fonts support enabled
- Frame-Options: DENY
- Content-Type-Options: nosniff
- XSS-Protection: disabled (modern browsers handle this)

### Central Package Management

Package versions are managed centrally in `Directory.Packages.props`:

- All projects reference packages without versions
- Version consistency across the solution
- Easy package updates and vulnerability management

## 📚 Documentation

- **[Architecture Guide](docs/ARCHITECTURE.md)** - Detailed architecture patterns and design decisions
- **[Security Guide](docs/SECURITY.md)** - Security implementation and best practices
- **[Configuration Guide](docs/CONFIGURATION.md)** - Configuration management and options
- **[Testing Guide](docs/TESTING.md)** - Testing strategies and examples
- **[Observability Guide](docs/OBSERVABILITY.md)** - Monitoring and telemetry setup
- **[Development Guide](docs/DEVELOPMENT.md)** - Local development and debugging

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit changes: `git commit -am 'Add my feature'`
4. Push to branch: `git push origin feature/my-feature`
5. Submit a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🏢 Enterprise Ready

This template is production-ready and includes:

- Security best practices and headers
- Comprehensive error handling and logging
- Performance monitoring and health checks
- Automated testing and CI/CD pipeline
- Scalable architecture patterns
- Configuration management for multiple environments

Perfect for enterprise applications requiring modern web development practices with security, observability, and maintainability.
