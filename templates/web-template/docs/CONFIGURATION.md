# Configuration Guide

This document explains the configuration patterns, Central Package Management, and environment-specific settings used in the Enterprise UI Template.

## 📋 Configuration Overview

The Enterprise UI Template uses modern .NET configuration patterns with:

1. **Central Package Management (CPM)**: Centralized dependency version management
2. **Strongly-Typed Configuration**: Type-safe configuration binding
3. **Environment-Specific Settings**: Different configs for dev/staging/production
4. **Hierarchical Configuration**: Layered configuration sources
5. **Secret Management**: Secure handling of sensitive data

## 📦 Central Package Management (CPM)

### What is CPM?

Central Package Management centralizes all NuGet package versions in a single file, ensuring version consistency across all projects in the solution.

### Directory.Packages.props

The root configuration file for CPM:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <!-- UI Framework -->
  <ItemGroup>
    <PackageVersion Include="MudBlazor" Version="7.8.0" />
    <PackageVersion Include="MudBlazor.ThemeManager" Version="7.8.0" />
  </ItemGroup>

  <!-- Authentication -->
  <ItemGroup>
    <PackageVersion Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="9.0.0" />
    <PackageVersion Include="Microsoft.AspNetCore.Authentication.Cookies" Version="2.2.0" />
  </ItemGroup>

  <!-- HTTP & Resilience -->
  <ItemGroup>
    <PackageVersion Include="Microsoft.Extensions.Http.Polly" Version="9.0.0" />
    <PackageVersion Include="Polly" Version="8.4.2" />
    <PackageVersion Include="Polly.Extensions.Http" Version="3.0.0" />
  </ItemGroup>

  <!-- Observability -->
  <ItemGroup>
    <PackageVersion Include="OpenTelemetry" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Exporter.Console" Version="1.9.0" />
  </ItemGroup>

  <!-- API Client Generation -->
  <ItemGroup>
    <PackageVersion Include="NSwag.MSBuild" Version="14.1.0" />
    <PackageVersion Include="NSwag.Core" Version="14.1.0" />
  </ItemGroup>

  <!-- Testing -->
  <ItemGroup>
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageVersion Include="bunit" Version="1.28.9" />
    <PackageVersion Include="bunit.web" Version="1.28.9" />
    <PackageVersion Include="Microsoft.Playwright" Version="1.48.0" />
    <PackageVersion Include="Microsoft.Playwright.MSTest" Version="1.48.0" />
    <PackageVersion Include="FluentAssertions" Version="6.12.1" />
    <PackageVersion Include="NSubstitute" Version="5.1.0" />
    <PackageVersion Include="MSTest.TestAdapter" Version="3.6.0" />
    <PackageVersion Include="MSTest.TestFramework" Version="3.6.0" />
    <PackageVersion Include="coverlet.collector" Version="6.0.2" />
  </ItemGroup>
</Project>
```

### Benefits of CPM

1. **Version Consistency**: All projects use the same package versions
2. **Easier Updates**: Update versions in one place
3. **Reduced Conflicts**: Eliminates package version conflicts
4. **Cleaner Project Files**: No version numbers in individual .csproj files
5. **Better Security**: Easier to track and update vulnerable packages

### Project File Changes

With CPM enabled, project files reference packages without versions:

```xml
<!-- Before CPM -->
<PackageReference Include="MudBlazor" Version="7.8.0" />

<!-- After CPM -->
<PackageReference Include="MudBlazor" />
```

### Directory.Build.props

Global MSBuild properties applied to all projects:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <WarningsAsErrors />
    <WarningsNotAsErrors />
  </PropertyGroup>

  <!-- Development settings -->
  <PropertyGroup Condition="'$(Configuration)' == 'Debug'">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
  </PropertyGroup>

  <!-- Production settings -->
  <PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
  </PropertyGroup>
</Project>
```

## ⚙️ Application Configuration

### Configuration Sources Hierarchy

.NET loads configuration from multiple sources in order:

1. **appsettings.json** (base settings)
2. **appsettings.{Environment}.json** (environment-specific)
3. **User Secrets** (development only)
4. **Environment Variables**
5. **Command Line Arguments**

Later sources override earlier ones.

### appsettings.json (Base Configuration)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "Auth": {
    "Authority": "https://demo.duendesoftware.com",
    "ClientId": "enterprise-ui-client",
    "ApiScope": "api"
  },

  "HttpClients": {
    "ApiClient": {
      "BaseUrl": "https://localhost:8080/api/",
      "Timeout": "00:00:30",
      "RetryCount": 3,
      "CircuitBreakerFailureThreshold": 5,
      "CircuitBreakerSamplingDuration": "00:01:00"
    }
  },

  "FeatureFlags": {
    "EnableDarkMode": true,
    "EnableAdvancedFeatures": false,
    "EnableBetaFeatures": false
  },

  "Observability": {
    "ServiceName": "Enterprise.Ui",
    "ServiceVersion": "1.0.0",
    "EnableTracing": true,
    "EnableMetrics": true,
    "EnableLogging": true,
    "OtlpEndpoint": "http://localhost:4317"
  }
}
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },

  "Auth": {
    "Authority": "https://localhost:5001",
    "RequireHttpsMetadata": false
  },

  "HttpClients": {
    "ApiClient": {
      "BaseUrl": "https://localhost:7001/api/"
    }
  },

  "FeatureFlags": {
    "EnableBetaFeatures": true
  },

  "Observability": {
    "EnableConsoleExporter": true
  }
}
```

### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },

  "Auth": {
    "RequireHttpsMetadata": true
  },

  "HttpClients": {
    "ApiClient": {
      "Timeout": "00:01:00",
      "RetryCount": 5
    }
  },

  "Observability": {
    "EnableConsoleExporter": false,
    "OtlpEndpoint": "https://your-otel-collector.com:4317"
  }
}
```

## 🔧 Strongly-Typed Configuration

### Configuration Classes

Define strongly-typed configuration classes:

```csharp
// Configuration/AuthOptions.cs
public class AuthOptions
{
    public const string SectionName = "Auth";

    public string Authority { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string ApiScope { get; set; } = "";
    public bool RequireHttpsMetadata { get; set; } = true;
}

// Configuration/HttpClientOptions.cs
public class HttpClientOptions
{
    public const string SectionName = "HttpClients";

    public ApiClientOptions ApiClient { get; set; } = new();
}

public class ApiClientOptions
{
    public string BaseUrl { get; set; } = "";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int RetryCount { get; set; } = 3;
    public int CircuitBreakerFailureThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerSamplingDuration { get; set; } = TimeSpan.FromMinutes(1);
}

// Configuration/FeatureFlagsOptions.cs
public class FeatureFlagsOptions
{
    public const string SectionName = "FeatureFlags";

    public bool EnableDarkMode { get; set; } = true;
    public bool EnableAdvancedFeatures { get; set; } = false;
    public bool EnableBetaFeatures { get; set; } = false;
}

// Configuration/ObservabilityOptions.cs
public class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ServiceName { get; set; } = "Enterprise.Ui";
    public string ServiceVersion { get; set; } = "1.0.0";
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
    public string OtlpEndpoint { get; set; } = "";
    public bool EnableConsoleExporter { get; set; } = false;
}
```

### Configuration Registration

Register configuration classes in Program.cs:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register configuration sections
builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(AuthOptions.SectionName));

builder.Services.Configure<HttpClientOptions>(
    builder.Configuration.GetSection(HttpClientOptions.SectionName));

builder.Services.Configure<FeatureFlagsOptions>(
    builder.Configuration.GetSection(FeatureFlagsOptions.SectionName));

builder.Services.Configure<ObservabilityOptions>(
    builder.Configuration.GetSection(ObservabilityOptions.SectionName));

// Optional: Add validation
builder.Services.AddOptionsWithValidation<AuthOptions>(AuthOptions.SectionName);
```

### Using Configuration in Services

Inject configuration into services:

```csharp
public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<HttpClientOptions> _httpOptions;
    private readonly ILogger<ApiService> _logger;

    public ApiService(
        HttpClient httpClient,
        IOptions<HttpClientOptions> httpOptions,
        ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _httpOptions = httpOptions;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var options = _httpOptions.Value.ApiClient;

        using var cts = new CancellationTokenSource(options.Timeout);

        var response = await _httpClient.GetAsync(endpoint, cts.Token);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException();
    }
}
```

### Configuration Validation

Add validation attributes to configuration classes:

```csharp
public class AuthOptions
{
    public const string SectionName = "Auth";

    [Required]
    [Url]
    public string Authority { get; set; } = "";

    [Required]
    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    [Required]
    public string ApiScope { get; set; } = "";

    public bool RequireHttpsMetadata { get; set; } = true;
}

// Extension method for validation
public static class ConfigurationExtensions
{
    public static IServiceCollection AddOptionsWithValidation<T>(
        this IServiceCollection services,
        string sectionName) where T : class
    {
        return services
            .AddOptions<T>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
    }
}
```

## 🔐 Secret Management

### Development Secrets (User Secrets)

Use User Secrets for development:

```bash
# Initialize user secrets
dotnet user-secrets init --project src/Enterprise.App

# Set secrets
dotnet user-secrets set "Auth:ClientSecret" "dev-client-secret"
dotnet user-secrets set "ConnectionStrings:Database" "dev-connection-string"

# List secrets
dotnet user-secrets list --project src/Enterprise.App
```

User secrets are stored outside the project directory:

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\{user-secrets-id}\secrets.json`
- **Linux/macOS**: `~/.microsoft/usersecrets/{user-secrets-id}/secrets.json`

### Environment Variables

Set configuration via environment variables:

```bash
# PowerShell
$env:Auth__ClientSecret="production-secret"
$env:ConnectionStrings__Database="production-connection"

# Bash
export Auth__ClientSecret="production-secret"
export ConnectionStrings__Database="production-connection"
```

**Note**: Use double underscores (`__`) to represent nested configuration sections.

### Azure Key Vault (Production)

For production, use Azure Key Vault:

```csharp
// Program.cs
if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());
    }
}
```

Azure Key Vault naming conventions:

- **Configuration**: `Auth--ClientSecret` becomes `Auth:ClientSecret`
- **Connection Strings**: `ConnectionStrings--Database` becomes `ConnectionStrings:Database`

## 🌍 Environment-Specific Configuration

### Environment Detection

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    // Development-specific configuration
    builder.Services.AddDeveloperExceptionPage();
}
else if (builder.Environment.IsStaging())
{
    // Staging-specific configuration
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
}
else if (builder.Environment.IsProduction())
{
    // Production-specific configuration
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });
}
```

### Environment Variables

Set the environment:

```bash
# Development
$env:ASPNETCORE_ENVIRONMENT="Development"

# Staging
$env:ASPNETCORE_ENVIRONMENT="Staging"

# Production
$env:ASPNETCORE_ENVIRONMENT="Production"
```

### Environment-Specific Services

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnvironmentSpecificServices(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddScoped<IEmailService, MockEmailService>();
            services.AddScoped<IPaymentService, MockPaymentService>();
        }
        else
        {
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IPaymentService, StripePaymentService>();
        }

        return services;
    }
}
```

## 📊 Configuration Monitoring

### Configuration Health Checks

Monitor configuration validity:

```csharp
public class ConfigurationHealthCheck : IHealthCheck
{
    private readonly IOptions<AuthOptions> _authOptions;
    private readonly IOptions<HttpClientOptions> _httpOptions;

    public ConfigurationHealthCheck(
        IOptions<AuthOptions> authOptions,
        IOptions<HttpClientOptions> httpOptions)
    {
        _authOptions = authOptions;
        _httpOptions = httpOptions;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auth = _authOptions.Value;
            var http = _httpOptions.Value;

            // Validate required configuration
            if (string.IsNullOrEmpty(auth.Authority))
                return Task.FromResult(HealthCheckResult.Unhealthy("Auth.Authority is not configured"));

            if (string.IsNullOrEmpty(auth.ClientId))
                return Task.FromResult(HealthCheckResult.Unhealthy("Auth.ClientId is not configured"));

            if (string.IsNullOrEmpty(http.ApiClient.BaseUrl))
                return Task.FromResult(HealthCheckResult.Unhealthy("HttpClients.ApiClient.BaseUrl is not configured"));

            return Task.FromResult(HealthCheckResult.Healthy("Configuration is valid"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Configuration validation failed", ex));
        }
    }
}
```

### Configuration Logging

Log configuration on startup:

```csharp
public static class ConfigurationLogger
{
    public static void LogConfiguration(IConfiguration configuration, ILogger logger)
    {
        logger.LogInformation("=== Configuration Summary ===");

        // Log non-sensitive configuration
        logger.LogInformation("Environment: {Environment}",
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        logger.LogInformation("Auth Authority: {Authority}",
            configuration["Auth:Authority"]);

        logger.LogInformation("API Base URL: {BaseUrl}",
            configuration["HttpClients:ApiClient:BaseUrl"]);

        // Don't log sensitive values
        var hasClientSecret = !string.IsNullOrEmpty(configuration["Auth:ClientSecret"]);
        logger.LogInformation("Auth ClientSecret configured: {HasSecret}", hasClientSecret);
    }
}

// Usage in Program.cs
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    ConfigurationLogger.LogConfiguration(
        app.Configuration,
        app.Services.GetRequiredService<ILogger<Program>>());
}
```

## 🔧 Configuration Best Practices

### Security Best Practices

1. **Never commit secrets to source control**
2. **Use User Secrets for development**
3. **Use Azure Key Vault or similar for production**
4. **Validate configuration on startup**
5. **Log configuration (without secrets) on startup**

### Performance Best Practices

1. **Use IOptions\<T> for configuration that doesn't change**
2. **Use IOptionsMonitor\<T> for configuration that can change at runtime**
3. **Use IOptionsSnapshot\<T> for configuration that changes per request**
4. **Bind configuration once at startup**

### Maintainability Best Practices

1. **Use strongly-typed configuration classes**
2. **Group related settings in sections**
3. **Use consistent naming conventions**
4. **Provide sensible defaults**
5. **Document configuration options**

### Example Configuration Documentation

```csharp
/// <summary>
/// Configuration options for HTTP client behavior
/// </summary>
public class HttpClientOptions
{
    public const string SectionName = "HttpClients";

    /// <summary>
    /// Base URL for the API client (required)
    /// Example: "https://api.example.com/v1/"
    /// </summary>
    public string BaseUrl { get; set; } = "";

    /// <summary>
    /// Request timeout duration
    /// Default: 30 seconds
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Number of retry attempts for failed requests
    /// Default: 3
    /// Range: 0-10
    /// </summary>
    [Range(0, 10)]
    public int RetryCount { get; set; } = 3;
}
```

This configuration guide provides comprehensive patterns for managing configuration in enterprise .NET applications with Central Package Management, strongly-typed options, and secure secret management.
