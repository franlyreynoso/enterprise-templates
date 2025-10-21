# Backend Integration Guide

This document explains how to use the Enterprise Blazor UI Template with backend API integration features.

## Overview

The Enterprise Blazor UI Template now supports seamless integration with .NET backend APIs, particularly those generated from the Enterprise Clean Architecture Template. This integration provides:

- **Automatic API Client Generation** using NSwag from OpenAPI specifications
- **Authentication Coordination** with JWT/OIDC providers
- **HTTP Resilience Patterns** with retry policies and circuit breakers
- **Development Environment Setup** with Docker Compose
- **Health Check Integration** for backend services

## Template Parameters

### Backend Integration Parameters

| Parameter                   | Type   | Default                    | Description                                             |
| --------------------------- | ------ | -------------------------- | ------------------------------------------------------- |
| `--backend-integration`     | bool   | `false`                    | Enable backend API integration features                 |
| `--backend-url`             | string | `"https://localhost:7001"` | Backend API base URL                                    |
| `--auth-provider`           | choice | `"JWT"`                    | Authentication provider (`JWT`, `OIDC`, `AzureAD`)      |
| `--api-client-generation`   | choice | `"NSwag"`                  | API client strategy (`NSwag`, `HttpClient`, `RestEase`) |
| `--include-dev-environment` | bool   | `false`                    | Include Docker Compose for integrated development       |

### Template Presets with Backend Integration

| Preset          | Description                                            | Backend Integration |
| --------------- | ------------------------------------------------------ | ------------------- |
| `FullStack`     | Full enterprise UI with backend integration            | ✅ Enabled          |
| `APIIntegrated` | Standard features with enhanced API client integration | ✅ Enabled          |
| `Full`          | All enterprise features (standalone)                   | ❌ Disabled         |
| `Standard`      | Common enterprise features (standalone)                | ❌ Disabled         |

## Usage Examples

### Create FullStack Project

```bash
# Create a full-stack project with default settings
dotnet new blazor-enterprise -n MyCompany.MyProject \
  --template-preset FullStack

# Create with custom backend URL
dotnet new blazor-enterprise -n MyCompany.MyProject \
  --template-preset FullStack \
  --backend-url "https://api.mycompany.com"
```

### Create API Integrated Project

```bash
# Create with API integration and custom settings
dotnet new blazor-enterprise -n MyCompany.MyProject \
  --template-preset APIIntegrated \
  --backend-url "https://localhost:8080" \
  --auth-provider "OIDC" \
  --include-dev-environment true
```

### Create Custom Project with Backend Integration

```bash
dotnet new blazor-enterprise -n MyCompany.MyProject \
  --template-preset Custom \
  --backend-integration true \
  --backend-url "https://api.example.com" \
  --auth-provider "JWT" \
  --api-client-generation "NSwag" \
  --include-auth true \
  --include-http-resilience true \
  --include-observability true
```

## Configuration

### Backend Configuration Structure

The template generates configuration files with backend integration settings:

**appsettings.json** (Production):

```json
{
  "Backend": {
    "ApiUrl": "https://api.production.com",
    "Timeout": "00:00:30",
    "RetryCount": 3,
    "EnableCircuitBreaker": true
  },
  "Authentication": {
    "Provider": "JWT",
    "TokenForwarding": true,
    "Authority": "https://auth.production.com",
    "Audience": "api"
  },
  "ApiClient": {
    "GenerationType": "NSwag",
    "BaseUrl": "https://api.production.com",
    "SwaggerEndpoint": "https://api.production.com/swagger/v1/swagger.json"
  }
}
```

**appsettings.Development.json**:

```json
{
  "Backend": {
    "ApiUrl": "https://localhost:7001",
    "Timeout": "00:01:00",
    "RetryCount": 2,
    "EnableCircuitBreaker": false
  },
  "Authentication": {
    "RequireHttpsMetadata": false
  }
}
```

### Environment Variables

You can override configuration using environment variables:

```bash
# Backend API URL
export Backend__ApiUrl="https://localhost:8080"

# Authentication settings
export Authentication__Authority="https://localhost:7001"
export Authentication__Audience="api"

# API Client settings
export ApiClient__SwaggerEndpoint="https://localhost:8080/swagger/v1/swagger.json"
```

## API Client Generation

### NSwag Configuration

The template configures NSwag to generate strongly-typed API clients:

**nswag.json**:

```json
{
  "runtime": "Net90",
  "documentGenerator": {
    "fromSwagger": {
      "url": "BackendUrl/swagger/v1/swagger.json"
    }
  },
  "codeGenerators": {
    "csharp": {
      "namespace": "Company.SourceName.ApiClient",
      "className": "ApiClient",
      "generateClientInterfaces": true,
      "useBaseUrl": true
    }
  }
}
```

### Using Generated API Clients

```csharp
// Inject the API client
@inject IApiClient ApiClient

// Use in components
protected override async Task OnInitializedAsync()
{
    try
    {
        var users = await ApiClient.GetUsersAsync();
        // Handle the response
    }
    catch (ApiException ex)
    {
        // Handle API errors
        Logger.LogError(ex, "Failed to load users");
    }
}
```

## Authentication Integration

### JWT Bearer Authentication

When using JWT authentication, the template configures:

1. **Token Forwarding**: Automatically forwards user tokens to API calls
2. **Token Validation**: Validates JWT tokens from the backend authority
3. **Authorization Policies**: Maps backend roles to UI authorization

```csharp
// Program.cs configuration (auto-generated)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Authentication:RequireHttpsMetadata", true);
    });
```

### OIDC Integration

For OpenID Connect scenarios:

```csharp
// OIDC configuration
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.ClientId = builder.Configuration["Authentication:ClientId"];
        options.ResponseType = "code";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("api");
    });
```

## Development Environment

### Docker Compose Integration

When `--include-dev-environment` is enabled, the template includes Docker Compose configurations:

**docker-compose.dev.yml**:

```yaml
version: "3.8"

services:
  app:
    build: .
    ports:
      - "7000:8080"
      - "7001:8081"
    environment:
      - Backend__ApiUrl=http://api:8080
    depends_on:
      - api

  api:
    image: enterprise-api:latest
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

### Starting the Development Environment

```bash
# Start both UI and API services
docker-compose -f docker-compose.dev.yml up

# Start with build
docker-compose -f docker-compose.dev.yml up --build

# Start specific services
docker-compose -f docker-compose.dev.yml up app api
```

## Health Checks

The template automatically configures health checks for backend services:

```csharp
// Health check configuration (auto-generated)
builder.Services.AddHealthChecks()
    .AddCheck("backend-api",
        () => HealthCheckBackend(configuration["Backend:ApiUrl"]))
    .AddUrlGroup(new Uri($"{configuration["Backend:ApiUrl"]}/health"),
        "backend-health");
```

Access health checks at: `https://localhost:7000/health`

## Troubleshooting

### Common Issues

**1. API Client Generation Fails**

```bash
# Check if backend API is running and accessible
curl https://localhost:7001/swagger/v1/swagger.json

# Regenerate API client
cd src/Company.MyProject.ApiClient
dotnet build
```

**2. Authentication Errors**

```bash
# Check authentication configuration
dotnet user-secrets list

# Verify backend authority is accessible
curl https://localhost:7001/.well-known/openid_configuration
```

**3. CORS Issues**
Ensure your backend API allows requests from the UI origin:

```csharp
// Backend API configuration
app.UseCors(policy => policy
    .WithOrigins("https://localhost:7000", "https://localhost:7001")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
```

### Debug Configuration

Enable detailed logging for API client calls:

**appsettings.Development.json**:

```json
{
  "Logging": {
    "LogLevel": {
      "Company.MyProject.ApiClient": "Debug",
      "System.Net.Http.HttpClient": "Information"
    }
  }
}
```

## Best Practices

1. **Environment-Specific URLs**: Use different backend URLs for development, staging, and production
2. **Error Handling**: Implement proper error handling for API client calls
3. **Circuit Breakers**: Enable circuit breakers for production environments
4. **Health Monitoring**: Monitor both UI and backend health endpoints
5. **Security**: Always use HTTPS in production and validate SSL certificates
6. **Token Management**: Implement proper token refresh logic for long-running sessions

## Migration from Standalone Template

To add backend integration to an existing standalone project:

1. **Update Configuration**:

   ```bash
   # Add backend configuration fragments
   cp backend.json src/YourApp/Config/
   cp backend.Development.json src/YourApp/Config/
   ```

2. **Update NSwag Configuration**:

   ```json
   {
     "documentGenerator": {
       "fromSwagger": {
         "url": "https://your-api.com/swagger/v1/swagger.json"
       }
     }
   }
   ```

3. **Register Services**:

   ```csharp
   // Add to Program.cs
   builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
   {
       client.BaseAddress = new Uri(builder.Configuration["Backend:ApiUrl"]);
   });
   ```

4. **Regenerate Configuration**:
   ```bash
   ./setup.ps1
   ```

## Integration with Enterprise Clean Architecture Template

This UI template works seamlessly with the Enterprise Clean Architecture Template:

1. **Create Backend API**:

   ```bash
   dotnet new enterprise-clean -n MyCompany.MyProject.Api
   ```

2. **Create Frontend UI**:

   ```bash
   dotnet new blazor-enterprise -n MyCompany.MyProject.UI \
     --template-preset FullStack \
     --backend-url "https://localhost:7001"
   ```

3. **Configure Integration**:
   - Both templates share compatible authentication
   - Health checks work across services
   - Observability traces span both applications
   - Docker Compose orchestrates both services
