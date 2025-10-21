# Enterprise Blazor Template - AI Coding Instructions

## Architecture & Project Structure

This is a **modular enterprise Blazor Server template** with feature-based conditional compilation and centralized package management.

### Key Components

- **Main App**: `src/Enterprise.App/` - Blazor Server app with conditional features via `#if` directives
- **Feature Modules**: Each enterprise concern is a separate project (Auth, Http, Security, etc.)
- **Shared UI**: `src/Enterprise.Ui.Shared/` - Reusable components and MudBlazor theming
- **Central Packages**: `Directory.Packages.props` manages all package versions centrally

### Configuration System

**Critical Pattern**: This template uses **modular configuration fragments** instead of monolithic appsettings.json:

- Configuration fragments in `src/Enterprise.App/Config/` (auth.json, http.json, features.json, etc.)
- PowerShell merge script `merge-config.ps1` combines fragments based on enabled features
- Each fragment has Development overrides (e.g., `auth.Development.json`)

## Development Patterns

### Feature Modules (Extension Methods)

Each enterprise feature follows this pattern:

```csharp
// src/Enterprise.Ui.Auth/AuthExtensions.cs
public static IServiceCollection AddEnterpriseAuth(this IServiceCollection services, IConfiguration cfg)
```

**Usage in Program.cs**:

```csharp
#if (EnableAuth)
builder.Services.AddEnterpriseAuth(builder.Configuration);
#endif
```

### HTTP Client Pattern

- **Correlation IDs**: All HTTP requests automatically get `x-correlation-id` headers via `CorrelationHandler`
- **Polly Resilience**: Retry, timeout, and circuit breaker policies are pre-configured
- **Named Clients**: Register with `AddEnterpriseHttpClient()` and inject `IHttpClientFactory`

### Feature Flags

Simple configuration-based flags via `IFeatureFlagProvider`:

```csharp
var isEnabled = await featureFlags.IsEnabledAsync("NewNav");
```

### Testing Strategy

- **bUnit**: Component testing with `BaseTestContext` for MudBlazor service registration
- **Health Endpoints**: `/health`, `/health/ready`, `/health/live` for monitoring

## Commands & Workflows

### Environment-Based Development (Matches Backend Template)

```bash
# Environment-specific commands (uses docker-compose.envs.yml with profiles)
make up ENV=dev              # Start development environment
make up ENV=staging          # Start staging environment
make up ENV=prod            # Start production environment
make down ENV=dev           # Stop environment

# Quick shortcuts
make up-dev                 # Start development
make up-staging            # Start staging
make up-prod               # Start production
make up-integrated         # Start full-stack (UI + Backend + Database)
```

### Configuration Management

```bash
# Environment-specific configuration generation
make config ENV=dev         # Generate development config
make config ENV=staging     # Generate staging config
make config ENV=prod       # Generate production config

# PowerShell alternative
cd src/Enterprise.App
.\merge-config.ps1 -Environment Development -EnableAuth -EnableObservability

# Environment files (matches backend variable naming)
# .env.dev     - Development configuration with DB_USER, DB_PASS, etc.
# .env.staging - Staging configuration
# .env.prod    - Production configuration
```

### Observability & Logging (Matches Backend Stack)

```bash
make logs ENV=dev           # View environment logs
make logs-seq              # View centralized logging (Seq)
make logs-integrated       # View full-stack logs

# Observability endpoints:
# - Jaeger (tracing): http://localhost:16686
# - Seq (logging): http://localhost:5341
# - Grafana (metrics): http://localhost:3000
```

### Testing & Database

```bash
dotnet test               # bUnit component tests
make test-e2e            # End-to-end tests (auto-installs Playwright)

# PostgreSQL database access:
# - Connection: localhost:5432
# - pgAdmin: http://localhost:5050 (dev only)
```

## Code Conventions

### Service Registration

- Use extension methods for feature-specific service registration
- Follow naming: `AddEnterprise{Feature}()` pattern
- Always accept `IConfiguration` parameter for settings

### Security Headers

- CSP allows Google Fonts and localhost WebSocket in development
- Security headers applied via middleware, not attributes
- Development mode automatically adjusts CSP for hot reload

### Project References

- All projects use `<PackageReference Include="PackageName" />` (no Version attribute)
- Versions managed centrally in `Directory.Packages.props`
- Use `PrivateAssets="All"` for build-time tools (NSwag, analyzers)

### Health Checks

Custom health service pattern in `src/Enterprise.App/Services/HealthService.cs` for application-specific checks.

## Important Files to Understand

- `Program.cs` - Conditional feature registration with compiler directives
- `Directory.Packages.props` - Single source of truth for package versions
- `Config/*.json` - Modular configuration fragments
- `merge-config.ps1` - Configuration assembly automation
- `BaseTestContext.cs` - Testing setup for MudBlazor components
- `.env.dev/.env.staging/.env.prod` - Environment-specific configuration (matches backend variable names: DB_USER, DB_PASS, etc.)
- `Makefile` - Unified command system with PowerShell detection matching backend template exactly
- `docker-compose.envs.yml` - Single compose file with profiles ["dev", "staging", "prod", "integrated"] matching backend architecture
- `otel-collector-config.yaml` - OpenTelemetry collector configuration for tracing pipeline
