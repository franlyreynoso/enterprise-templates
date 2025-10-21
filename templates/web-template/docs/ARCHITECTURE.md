# Architecture Guide

This document provides a comprehensive overview of the Enterprise UI Template architecture, design patterns, and implementation decisions.

## 🏛️ Architectural Overview

### Design Principles

The Enterprise UI Template follows these core architectural principles:

1. **Separation of Concerns**: Clear boundaries between different layers and responsibilities
2. **Dependency Injection**: Loose coupling through IoC container and service interfaces
3. **Configuration-Driven**: External configuration for environment-specific settings
4. **Cross-Cutting Concerns**: Centralized handling of logging, security, and observability
5. **Testability**: Design for unit testing and integration testing
6. **Scalability**: Patterns that support horizontal and vertical scaling

### Layered Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   Pages     │  │ Components  │  │      Layouts        │  │
│  │ (Routing)   │  │   (UI)      │  │    (Templates)      │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             │
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │  Services   │  │  Commands   │  │     Handlers        │  │
│  │ (Business)  │  │ (Actions)   │  │   (Processing)      │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             │
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ HTTP Clients│  │   Auth      │  │    Configuration    │  │
│  │ (External)  │  │ (Identity)  │  │     (Settings)      │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                             │
┌─────────────────────────────────────────────────────────────┐
│                Cross-Cutting Concerns                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ Observability│  │  Security   │  │   Feature Flags     │  │
│  │   (Logs)    │  │  (Headers)  │  │    (Toggles)        │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## 🧩 Project Structure Deep Dive

### Core Application (`Enterprise.App`)

**Purpose**: Main Blazor Server application entry point and composition root.

**Key Components**:

- `Program.cs`: Application startup, service configuration, middleware pipeline
- `App.razor`: Root component with routing, layouts, and error handling
- `Components/Routes.razor`: Routing configuration with navigation logic
- `Pages/`: Page components mapped to URL routes
- `Services/`: Application-specific services (e.g., HealthService)

**Architecture Patterns**:

- **Composition Root**: Single location for dependency registration
- **Middleware Pipeline**: Ordered request processing with cross-cutting concerns
- **Component Hierarchy**: Nested component structure with data flow

### Shared UI Library (`Enterprise.Ui.Shared`)

**Purpose**: Reusable UI components, layouts, and theming infrastructure.

**Key Components**:

```
Enterprise.Ui.Shared/
├── Components/
│   ├── ErrorBoundary.razor     # Global error handling component
│   └── [Other Components]      # Reusable UI components
├── Layouts/
│   └── AppLayout.razor         # Main application layout
└── Theme/
    └── ThemeProvider.razor     # MudBlazor theme configuration
```

**Design Patterns**:

- **Component Composition**: Building complex UI from simple components
- **Theme Provider Pattern**: Centralized theming with cascading parameters
- **Error Boundary Pattern**: Graceful error handling in component tree

### Authentication (`Enterprise.Ui.Auth`)

**Purpose**: Authentication and authorization infrastructure.

**Key Features**:

- OIDC (OpenID Connect) integration
- Role-based authorization
- Token management and forwarding
- Custom authentication extensions

**Implementation**:

```csharp
public static class AuthExtensions
{
    public static IServiceCollection AddEnterpriseAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure OIDC authentication
        // Setup authorization policies
        // Register authentication handlers
    }
}
```

### HTTP Infrastructure (`Enterprise.Ui.Http`)

**Purpose**: HTTP client configuration with resilience patterns.

**Key Features**:

- **Typed HTTP Clients**: Strongly-typed API clients
- **Polly Integration**: Retry policies, circuit breakers, timeouts
- **Authentication Forwarding**: Automatic token propagation
- **Correlation IDs**: Request tracking across services

**Resilience Patterns**:

```csharp
// Retry Policy Example
services.AddHttpClient<ApiClient>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

### Feature Flags (`Enterprise.Ui.FeatureFlags`)

**Purpose**: Runtime feature toggling and A/B testing infrastructure.

**Provider Pattern**:

```csharp
public interface IFeatureFlagProvider
{
    bool IsEnabled(string flagName);
    T GetValue<T>(string flagName, T defaultValue);
}

public class ConfigFeatureFlags : IFeatureFlagProvider
{
    // Configuration-based implementation
}
```

**Usage Patterns**:

- Configuration-driven feature toggles
- Component-level feature gates
- Service-level feature switches

### Observability (`Enterprise.Ui.Observability`)

**Purpose**: Monitoring, logging, and telemetry infrastructure.

**OpenTelemetry Integration**:

```csharp
services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());
```

**Monitoring Stack**:

- **Distributed Tracing**: Request flow across services
- **Metrics Collection**: Performance counters and custom metrics
- **Health Checks**: Application and dependency health monitoring
- **Structured Logging**: JSON-formatted logs with correlation context

### Security (`Enterprise.Ui.Security`)

**Purpose**: Security headers, CSP, and security middleware.

**Security Headers Implementation**:

```csharp
public static IApplicationBuilder UseEnterpriseSecurityHeaders(
    this IApplicationBuilder app, bool dev = false)
{
    return app.Use(async (ctx, next) =>
    {
        // X-Content-Type-Options: nosniff
        // X-Frame-Options: DENY
        // Referrer-Policy: strict-origin-when-cross-origin
        // Content-Security-Policy: [custom policy]

        await next();
    });
}
```

**Content Security Policy**:

- Restrictive default policy
- Google Fonts allowlisting
- Development vs. production differences
- Script and style source controls

## 🔄 Request Processing Flow

### Typical Request Lifecycle

1. **HTTP Request**: Browser sends request to Blazor Server
2. **Middleware Pipeline**: Request passes through security, auth, etc.
3. **Routing**: URL mapped to appropriate page component
4. **Component Rendering**: Server-side component rendering
5. **SignalR**: Real-time updates via WebSocket connection
6. **Response**: HTML/JSON sent back to browser

### Middleware Pipeline Order

```csharp
app.UseHttpsRedirection();          // 1. Force HTTPS
app.UseStaticFiles();               // 2. Serve static content
app.UseRequestLocalization();       // 3. Localization
app.UseAuthentication();            // 4. Authentication
app.UseAuthorization();             // 5. Authorization
app.UseAntiforgery();              // 6. CSRF protection
app.UseEnterpriseSecurityHeaders(); // 7. Security headers
app.MapRazorComponents<App>();      // 8. Blazor components
```

## 🏗️ Design Patterns

### Dependency Injection Patterns

**Service Registration**:

```csharp
// Scoped: Per HTTP request
services.AddScoped<IHealthService, HealthService>();

// Singleton: Application lifetime
services.AddSingleton<IFeatureFlagProvider, ConfigFeatureFlags>();

// Transient: New instance each time
services.AddTransient<ITemporaryService, TemporaryService>();
```

**Configuration Pattern**:

```csharp
// Strongly-typed configuration
services.Configure<ApiSettings>(
    configuration.GetSection("Api"));

// Usage in components
[Inject] IOptions<ApiSettings> ApiSettings { get; set; }
```

### Component Patterns

**Parameter Binding**:

```csharp
[Parameter] public string Title { get; set; } = "";
[Parameter] public RenderFragment? ChildContent { get; set; }
[Parameter] public EventCallback<string> OnValueChanged { get; set; }
```

**Error Boundary Pattern**:

```csharp
@inherits ErrorBoundaryBase

@if (CurrentException is null)
{
    @ChildContent
}
else
{
    <ErrorDisplay Exception="CurrentException" />
}
```

### HTTP Client Patterns

**Typed Client Pattern**:

```csharp
public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}
```

## 🔧 Configuration Architecture

### Central Package Management

**Directory.Packages.props**:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>

  <ItemGroup>
    <PackageVersion Include="MudBlazor" Version="7.8.0" />
    <PackageVersion Include="OpenTelemetry" Version="1.9.0" />
  </ItemGroup>
</Project>
```

**Benefits**:

- Version consistency across projects
- Easy dependency updates
- Centralized vulnerability management
- Reduced package reference duplication

### Environment Configuration

**appsettings.json Hierarchy**:

```
appsettings.json                    # Base configuration
├── appsettings.Development.json    # Development overrides
├── appsettings.Staging.json        # Staging overrides
└── appsettings.Production.json     # Production overrides
```

**Configuration Binding**:

```csharp
public class ApiSettings
{
    public string BaseUrl { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 30;
    public RetrySettings Retry { get; set; } = new();
}
```

## 🧪 Testing Architecture

### Component Testing (bUnit)

**Unit Test Structure**:

```csharp
[Test]
public void Component_Should_RenderCorrectly()
{
    // Arrange
    using var ctx = new TestContext();
    ctx.Services.AddSingleton<IFeatureFlagProvider>(mockProvider);

    // Act
    var component = ctx.RenderComponent<MyComponent>();

    // Assert
    component.Find("h1").TextContent.Should().Be("Expected Title");
}
```

### Integration Testing (Playwright)

**E2E Test Structure**:

```csharp
[Test]
public async Task Should_NavigateToHealthPage()
{
    await Page.GotoAsync("/health");
    await Page.ClickAsync("button:has-text('Check Application Health')");
    await Expect(Page.Locator(".mud-alert-success")).ToBeVisibleAsync();
}
```

## 🚀 Deployment Architecture

### Production Considerations

**Performance Optimizations**:

- Blazor Server render mode for optimal performance
- Static file compression and caching
- CDN integration for static assets
- Connection pooling for HTTP clients

**Security Hardening**:

- HTTPS enforcement
- Security headers (CSP, HSTS, etc.)
- Authentication token validation
- Input sanitization and validation

**Monitoring & Observability**:

- Application Performance Monitoring (APM)
- Health check endpoints for load balancers
- Distributed tracing for microservices
- Centralized logging with correlation IDs

## 📈 Scalability Patterns

### Horizontal Scaling

**Load Balancer Configuration**:

- Session affinity for SignalR connections
- Health check endpoints (`/health/ready`)
- Graceful shutdown handling

**Database Considerations**:

- Connection pooling optimization
- Read replicas for query scaling
- Caching layers (Redis, in-memory)

### Vertical Scaling

**Resource Optimization**:

- Memory usage monitoring
- CPU utilization tracking
- Connection limit management
- Garbage collection tuning

This architecture provides a solid foundation for enterprise applications with proper separation of concerns, testability, security, and scalability built in from the ground up.
