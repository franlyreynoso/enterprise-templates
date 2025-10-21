# Company.SourceName

A modern enterprise Blazor Server application built with the Enterprise UI Template.

## 🚀 Getting Started

### Prerequisites

- **.NET 9.0 SDK**
- **Visual Studio 2024** or **Visual Studio Code** with C# extension
<!--#if (EnableTesting)-->
- **Node.js 18+** (for Playwright end-to-end tests)
<!--#endif-->

### Quick Start

1. **Run the setup script to configure your project:**

   ```bash
   ./setup.ps1
   ```

   This will:

   - Generate appropriate `appsettings.json` files based on selected features
   - Show detected features and next steps
   - Provide configuration guidance

2. **Run the application:**

   ```bash
   cd src/Enterprise.App
   dotnet run
   ```

3. **Navigate to:** https://localhost:7000

> **Note:** The setup script automatically detects which features are enabled and generates clean configuration files with only the settings you need.

<!--#if (EnableTesting)-->

### Running Tests

```bash
# Unit tests (bUnit)
dotnet test tests/Enterprise.Ui.Tests/

# End-to-end tests (Playwright)
cd tests/Enterprise.Ui.Tests.E2E
npm install
npx playwright test
```

<!--#endif-->

## 🏗️ Architecture

This application is built using modular enterprise libraries:

- **Enterprise.App** - Main Blazor Server application
<!--#if (EnableAuth)-->
- **Enterprise.Ui.Auth** - Authentication & authorization
  <!--#endif-->
  <!--#if (EnableHttpResilience)-->
- **Enterprise.Ui.Http** - HTTP resilience with Polly
  <!--#endif-->
  <!--#if (EnableApiClient)-->
- **Enterprise.Ui.ApiClient** - Generated API clients
  <!--#endif-->
  <!--#if (EnableObservability)-->
- **Enterprise.Ui.Observability** - OpenTelemetry monitoring
  <!--#endif-->
  <!--#if (EnableFeatureFlags)-->
- **Enterprise.Ui.FeatureFlags** - Feature flag management
  <!--#endif-->
  <!--#if (EnableSecurity)-->
- **Enterprise.Ui.Security** - Security headers & CSP
<!--#endif-->
- **Enterprise.Ui.Shared** - Shared UI components and layouts

## ⚙️ Configuration

This project uses an intelligent configuration management system with modular fragments:

<!--#if (EnableAuth)-->

### Authentication

Configure your identity provider in `appsettings.json`:

```json
{
  "Auth": {
    "Authority": "https://your-identity-provider.com",
    "ClientId": "your-client-id",
    "ApiScope": "api"
  }
}
```

<!--#endif-->

<!--#if (EnableHttpResilience)-->

### HTTP Clients

Configure API endpoints and resilience policies:

```json
{
  "HttpClients": {
    "ApiClient": {
      "BaseUrl": "https://your-api.com/",
      "Timeout": "00:00:30",
      "RetryCount": 3
    }
  }
}
```

<!--#endif-->

<!--#if (EnableObservability)-->

### Observability

Configure OpenTelemetry for monitoring:

```json
{
  "Observability": {
    "ServiceName": "Company.SourceName",
    "OtlpEndpoint": "http://your-otel-collector:4317",
    "EnableTracing": true,
    "EnableMetrics": true
  }
}
```

<!--#endif-->

For complete configuration documentation, see [CONFIG.md](src/Enterprise.App/CONFIG.md).

## 🔧 Development

### Adding New Features

The application is designed for extensibility. To add new features:

1. **Create a new library project** in `src/Enterprise.Ui.YourFeature/`
2. **Add extension methods** to register services
3. **Update Program.cs** to call your extensions
4. **Add configuration** using the modular fragment system

### Feature Flags

<!--#if (EnableFeatureFlags)-->

This application includes feature flags for controlled feature rollouts:

```csharp
@inject IFeatureFlags Features

@if (await Features.IsEnabledAsync("EnableNewFeature"))
{
    <NewFeatureComponent />
}
```

Configure flags in `appsettings.json`:

```json
{
  "FeatureFlags": {
    "EnableNewFeature": true,
    "EnableBetaFeatures": false
  }
}
```

<!--#endif-->
<!--#if (!EnableFeatureFlags)-->

Feature flags are not enabled in this project. To add feature flag support, reinstall the template with `--IncludeFeatureFlags`.

<!--#endif-->

### Health Checks

The application includes comprehensive health checks at `/health`:

- **Application Health** - Overall application status
<!--#if (EnableAuth)-->
- **Authentication** - Identity provider connectivity
  <!--#endif-->
  <!--#if (EnableHttpResilience)-->
- **External APIs** - Downstream service availability
  <!--#endif-->
  <!--#if (EnableObservability)-->
- **Telemetry** - OpenTelemetry exporter status
<!--#endif-->

## 🚀 Deployment

<!--#if (EnableDocker)-->

### Docker

Build and run with Docker:

```bash
# Build image
docker build -t company-sourcename .

# Run container
docker run -p 8080:8080 company-sourcename

# Or use docker-compose
docker-compose up
```

<!--#endif-->

<!--#if (EnableCICD)-->

### CI/CD

This project includes GitHub Actions workflows for:

- **Continuous Integration** - Build, test, and quality checks
- **Security Scanning** - Dependency and code security analysis
- **Automated Deployment** - Deploy to staging and production environments

See `.github/workflows/` for configuration details.

<!--#endif-->

### Environment Variables

Key environment variables for deployment:

<!--#if (EnableAuth)-->

- `Auth__Authority` - Identity provider URL
- `Auth__ClientId` - OAuth2/OIDC client identifier
  <!--#endif-->
  <!--#if (EnableObservability)-->
- `Observability__OtlpEndpoint` - OpenTelemetry collector endpoint
<!--#endif-->
- `ASPNETCORE_ENVIRONMENT` - Environment name (Development, Staging, Production)

## 📚 Resources

- [Enterprise UI Template Documentation](https://github.com/your-org/enterprise-ui-template)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [MudBlazor Components](https://mudblazor.com/)
<!--#if (EnableObservability)-->
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
  <!--#endif-->
  <!--#if (EnableAuth)-->
- [ASP.NET Core Authentication](https://docs.microsoft.com/aspnet/core/security/authentication/)
<!--#endif-->

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Commit your changes** (`git commit -m 'Add amazing feature'`)
4. **Push to the branch** (`git push origin feature/amazing-feature`)
5. **Open a Pull Request**

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
