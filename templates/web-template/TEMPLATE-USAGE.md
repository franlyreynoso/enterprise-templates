# Enterprise Blazor UI Template

## 🎯 Template Usage Guide

This configurable .NET template allows you to create enterprise Blazor applications with selectively enabled features. Choose from predefined presets or customize individual features to match your project requirements.

## 🚀 Quick Start

### Install the Template

```bash
# Install from local folder
dotnet new install ./

# Or install from NuGet (when published)
dotnet new install Enterprise.Blazor.Template
```

### Create a New Project

#### Using Presets (Recommended)

```bash
# Full enterprise application (all features enabled)
dotnet new blazor-enterprise -n MyApp --TemplatePreset Full

# Minimal Blazor app with MudBlazor UI only
dotnet new blazor-enterprise -n MyApp --TemplatePreset Minimal

# Standard enterprise features (no advanced observability)
dotnet new blazor-enterprise -n MyApp --TemplatePreset Standard

# Microservice-optimized features
dotnet new blazor-enterprise -n MyApp --TemplatePreset Microservice
```

#### Custom Feature Selection

```bash
# Custom selection with specific features
dotnet new blazor-enterprise -n MyApp \
  --TemplatePreset Custom \
  --IncludeAuth true \
  --IncludeHttpResilience true \
  --IncludeObservability false \
  --IncludeTesting true
```

## 📋 Template Presets

### 🏢 Full (Production Ready)

**Best for: Production enterprise applications**

✅ Authentication & Authorization (OIDC)
✅ HTTP Client with Resilience Patterns
✅ API Client Generation (NSwag)
✅ OpenTelemetry Observability
✅ Feature Flags
✅ Security Headers & CSP
✅ Internationalization
✅ Testing Infrastructure (bUnit, Playwright)
✅ CI/CD Pipeline (GitHub Actions)
✅ Docker Support

```bash
dotnet new blazor-enterprise -n MyEnterpriseApp --TemplatePreset Full
```

### 🎯 Standard (Common Enterprise)

**Best for: Most business applications**

✅ Authentication & Authorization
✅ HTTP Client with Resilience
✅ API Client Generation
✅ Feature Flags
✅ Security Headers
✅ Testing Infrastructure
✅ CI/CD Pipeline
❌ Advanced Observability
❌ Internationalization
❌ Docker Support

```bash
dotnet new blazor-enterprise -n MyBusinessApp --TemplatePreset Standard
```

### 🏗️ Microservice (Service Architecture)

**Best for: Microservice/distributed applications**

✅ Authentication & Authorization
✅ HTTP Client with Resilience
✅ API Client Generation
✅ OpenTelemetry Observability
✅ Feature Flags
✅ Security Headers
✅ Docker Support
❌ Internationalization
❌ Full Testing Suite
❌ CI/CD Pipeline (handled at orchestration level)

```bash
dotnet new blazor-enterprise -n MyMicroservice --TemplatePreset Microservice
```

### 🎨 Minimal (UI Focus)

**Best for: Simple applications, prototypes, UI showcases**

✅ Basic Blazor Server
✅ MudBlazor UI Components
✅ Dark/Light Theme Support
❌ All enterprise features disabled

```bash
dotnet new blazor-enterprise -n MySimpleApp --TemplatePreset Minimal
```

## 🔧 Individual Feature Flags

### Core Features

| Feature             | Flag                      | Description                                       |
| ------------------- | ------------------------- | ------------------------------------------------- |
| **Authentication**  | `--IncludeAuth`           | OIDC authentication with role-based authorization |
| **HTTP Resilience** | `--IncludeHttpResilience` | Polly retry policies, circuit breakers, timeouts  |
| **API Client**      | `--IncludeApiClient`      | NSwag-generated typed API clients                 |
| **Observability**   | `--IncludeObservability`  | OpenTelemetry tracing, metrics, and logging       |
| **Feature Flags**   | `--IncludeFeatureFlags`   | Configuration-based feature toggles               |
| **Security**        | `--IncludeSecurity`       | Security headers, CSP, HTTPS enforcement          |

### Optional Features

| Feature                  | Flag               | Description                                    |
| ------------------------ | ------------------ | ---------------------------------------------- |
| **Internationalization** | `--IncludeI18n`    | Multi-language support and localization        |
| **Testing**              | `--IncludeTesting` | bUnit component tests and Playwright E2E tests |
| **CI/CD**                | `--IncludeCICD`    | GitHub Actions workflows                       |
| **Docker**               | `--IncludeDocker`  | Containerization support                       |

## 💡 Example Usage Scenarios

### Scenario 1: Internal Business Application

```bash
dotnet new blazor-enterprise -n InternalApp \
  --TemplatePreset Custom \
  --IncludeAuth true \
  --IncludeHttpResilience true \
  --IncludeFeatureFlags true \
  --IncludeSecurity true \
  --IncludeTesting true \
  --IncludeObservability false \
  --IncludeDocker false
```

### Scenario 2: Customer-Facing Portal

```bash
dotnet new blazor-enterprise -n CustomerPortal \
  --TemplatePreset Custom \
  --IncludeAuth true \
  --IncludeHttpResilience true \
  --IncludeApiClient true \
  --IncludeSecurity true \
  --IncludeI18n true \
  --IncludeObservability true \
  --IncludeTesting true \
  --IncludeCICD true
```

### Scenario 3: Microservice UI

```bash
dotnet new blazor-enterprise -n OrderUI \
  --TemplatePreset Microservice
```

### Scenario 4: Prototype/Demo App

```bash
dotnet new blazor-enterprise -n DemoApp \
  --TemplatePreset Minimal
```

## 🏗️ Generated Project Structure

The template generates different project structures based on selected features:

### Full Template Structure

```
MyApp/
├── src/
│   ├── MyApp/                          # Main Blazor application
│   ├── MyApp.Ui.Shared/               # Shared UI components (always included)
│   ├── MyApp.Ui.Auth/                 # Authentication (if IncludeAuth)
│   ├── MyApp.Ui.Http/                 # HTTP resilience (if IncludeHttpResilience)
│   ├── MyApp.Ui.ApiClient/            # API clients (if IncludeApiClient)
│   ├── MyApp.Ui.Observability/        # OpenTelemetry (if IncludeObservability)
│   ├── MyApp.Ui.FeatureFlags/         # Feature flags (if IncludeFeatureFlags)
│   └── MyApp.Ui.Security/             # Security headers (if IncludeSecurity)
├── tests/                             # Test projects (if IncludeTesting)
├── docs/                              # Documentation (always included)
├── .github/workflows/                 # CI/CD pipelines (if IncludeCICD)
├── Dockerfile                         # Container support (if IncludeDocker)
├── docker-compose.dev.yml             # Development containers (if IncludeDocker)
└── MyApp.sln                          # Solution file
```

### Minimal Template Structure

```
MyApp/
├── src/
│   ├── MyApp/                          # Main Blazor application
│   └── MyApp.Ui.Shared/               # Shared UI components
├── docs/                              # Basic documentation
└── MyApp.sln                          # Solution file
```

## 🔧 Post-Generation Steps

### 1. Configure Authentication (if enabled)

Update `appsettings.json` with your OIDC provider:

```json
{
  "Auth": {
    "Authority": "https://your-identity-provider.com/",
    "ClientId": "your-client-id",
    "ApiScope": "your-api-scope"
  }
}
```

### 2. Set Up User Secrets (if enabled)

```bash
dotnet user-secrets init --project src/MyApp
dotnet user-secrets set "Auth:ClientSecret" "your-secret"
```

### 3. Configure API Endpoints (if API client enabled)

Update the NSwag configuration in `src/MyApp.Ui.ApiClient/nswag.json`

### 4. Set Up Observability (if enabled)

Configure OpenTelemetry endpoints in `appsettings.json`:

```json
{
  "Observability": {
    "OtlpEndpoint": "http://your-collector:4317"
  }
}
```

## 🚀 Running Your Application

```bash
# Restore dependencies and run
dotnet restore
dotnet run --project src/MyApp

# Or with Docker (if enabled)
docker-compose -f docker-compose.dev.yml up
```

## 📚 Feature Documentation

Each generated project includes comprehensive documentation in the `docs/` folder:

- **ARCHITECTURE.md** - Technical architecture and design patterns
- **SECURITY.md** - Security implementation and best practices
- **CONFIGURATION.md** - Configuration management and settings
- **TESTING.md** - Testing strategies and examples
- **OBSERVABILITY.md** - Monitoring and observability setup
- **DEVELOPMENT.md** - Development workflow and contribution guidelines

## 🔄 Template Updates

### Update Template

```bash
# Uninstall old version
dotnet new uninstall Enterprise.Blazor.Template

# Install new version
dotnet new install Enterprise.Blazor.Template::2.0.0
```

### List Installed Templates

```bash
dotnet new list
```

## 🆘 Support & Troubleshooting

### Common Issues

**Missing Dependencies:**

```bash
dotnet restore
dotnet build
```

**Template Not Found:**

```bash
dotnet new install ./path/to/template
```

**Feature Conflicts:**
Some feature combinations are automatically validated. The template will warn you about incompatible selections.

### Help

```bash
# Show template help
dotnet new blazor-enterprise --help

# List all available options
dotnet new blazor-enterprise --dry-run
```

## 🎯 Next Steps

1. **Configure your selected features** using the generated documentation
2. **Customize the UI** by modifying components in `Enterprise.Ui.Shared`
3. **Add your business logic** to the main application project
4. **Set up CI/CD** using the generated GitHub Actions workflows
5. **Deploy** using the provided Docker configuration or your preferred hosting platform

The Enterprise Blazor UI Template provides a solid foundation for building modern, scalable web applications with enterprise-grade patterns and practices. Choose the features that match your requirements and start building! 🚀
