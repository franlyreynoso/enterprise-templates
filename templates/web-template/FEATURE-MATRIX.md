# Template Feature Compatibility Matrix

# This file documents feature dependencies and recommendations

## Feature Dependencies

### Core Dependencies

- **Enterprise.Ui.Shared**: Always required (base UI components)
- **MudBlazor**: Always included (UI framework)

### Authentication Dependencies

- **EnableAuth** → No additional dependencies
- **Note**: Works best with EnableSecurity for complete auth flow

### HTTP Resilience Dependencies

- **EnableHttpResilience** → Polly packages
- **Recommendation**: Combine with EnableObservability for monitoring resilience patterns

### API Client Dependencies

- **EnableApiClient** → NSwag packages
- **Recommendation**: Combine with EnableHttpResilience for robust API calls

### Observability Dependencies

- **EnableObservability** → OpenTelemetry packages
- **Recommendation**: Essential for production applications
- **Docker Integration**: Automatically includes observability stack in docker-compose

### Security Dependencies

- **EnableSecurity** → No additional dependencies
- **Recommendation**: Should always be enabled for production

### Feature Flags Dependencies

- **EnableFeatureFlags** → No additional dependencies
- **Note**: Uses configuration-based implementation

### Internationalization Dependencies

- **EnableI18n** → Localization packages
- **Note**: Adds complexity, only enable if truly needed

### Testing Dependencies

- **EnableTesting** → bUnit, Playwright packages
- **Recommendation**: Always enable for maintainable applications

### CI/CD Dependencies

- **EnableCICD** → No additional dependencies
- **Note**: Creates GitHub Actions workflows

### Docker Dependencies

- **EnableDocker** → No additional dependencies
- **Integration**: Automatically configures observability stack if enabled

## Preset Configurations

### Full Preset (Production Ready)

```
EnableAuth = true
EnableHttpResilience = true
EnableApiClient = true
EnableObservability = true
EnableFeatureFlags = true
EnableSecurity = true
EnableI18n = true
EnableTesting = true
EnableCICD = true
EnableDocker = true
```

**Best for**: Production enterprise applications requiring all features

### Standard Preset (Common Enterprise)

```
EnableAuth = true
EnableHttpResilience = true
EnableApiClient = true
EnableObservability = false  # Simplified monitoring
EnableFeatureFlags = true
EnableSecurity = true
EnableI18n = false           # English only
EnableTesting = true
EnableCICD = true
EnableDocker = false         # Traditional deployment
```

**Best for**: Most business applications with standard enterprise needs

### Microservice Preset (Service Architecture)

```
EnableAuth = true
EnableHttpResilience = true
EnableApiClient = true
EnableObservability = true   # Essential for distributed systems
EnableFeatureFlags = true
EnableSecurity = true
EnableI18n = false
EnableTesting = false        # Testing handled at integration level
EnableCICD = false           # CI/CD handled at orchestration level
EnableDocker = true          # Container-first deployment
```

**Best for**: Microservice architectures with external orchestration

### Minimal Preset (UI Focus)

```
All features = false
```

**Best for**: Simple applications, prototypes, UI demos

## Feature Combinations to Avoid

### ❌ Problematic Combinations

1. **EnableAuth=false + EnableSecurity=false**

   - Security risk for any non-demo application
   - Consider at least enabling security headers

2. **EnableObservability=false + Microservice=true**

   - Debugging distributed systems becomes very difficult
   - Strongly recommend enabling observability for microservices

3. **EnableHttpResilience=false + EnableApiClient=true**
   - API calls without resilience patterns
   - Consider enabling resilience for production stability

### ⚠️ Use with Caution

1. **EnableI18n=true + Minimal feature set**

   - Adds significant complexity for simple applications
   - Only enable if internationalization is truly required

2. **EnableDocker=true + EnableCICD=false**

   - Manual container deployment workflow
   - Consider adding CI/CD for automated deployments

3. **EnableTesting=false + Production use**
   - Reduced maintainability and reliability
   - Strongly recommend including testing infrastructure

## Recommended Combinations

### 🚀 Rapid Prototyping

```bash
dotnet new blazor-enterprise -n Prototype \
  --TemplatePreset Custom \
  --IncludeAuth false \
  --IncludeHttpResilience false \
  --IncludeObservability false \
  --IncludeTesting false \
  --IncludeCICD false
```

### 🏢 Internal Business App

```bash
dotnet new blazor-enterprise -n BusinessApp \
  --TemplatePreset Custom \
  --IncludeAuth true \
  --IncludeHttpResilience true \
  --IncludeFeatureFlags true \
  --IncludeSecurity true \
  --IncludeTesting true \
  --IncludeObservability false
```

### 🌐 Customer Portal

```bash
dotnet new blazor-enterprise -n CustomerPortal \
  --TemplatePreset Full
```

### 🔧 API Gateway UI

```bash
dotnet new blazor-enterprise -n ApiGatewayUI \
  --TemplatePreset Custom \
  --IncludeAuth true \
  --IncludeHttpResilience true \
  --IncludeApiClient true \
  --IncludeObservability true \
  --IncludeSecurity true \
  --IncludeDocker true
```

## Package Impact by Feature

### Minimal Packages (Always Included)

- Microsoft.AspNetCore.App
- MudBlazor + MudBlazor.ThemeManager

### Additional Packages by Feature

**Authentication (+6 packages)**

- Microsoft.AspNetCore.Authentication.OpenIdConnect
- Microsoft.AspNetCore.Authentication.Cookies

**HTTP Resilience (+3 packages)**

- Microsoft.Extensions.Http.Polly
- Polly + Polly.Extensions.Http

**API Client (+2 packages)**

- NSwag.MSBuild
- NSwag.Core

**Observability (+5 packages)**

- OpenTelemetry.\*
- Multiple OpenTelemetry instrumentation packages

**Testing (+8 packages)**

- bUnit + bunit.web
- Microsoft.Playwright + Microsoft.Playwright.MSTest
- FluentAssertions, NSubstitute, MSTest packages

## Project Size Impact

| Configuration | Projects | Approx Lines of Code | Docker Image Size |
| ------------- | -------- | -------------------- | ----------------- |
| Minimal       | 2        | 500                  | 180MB             |
| Standard      | 7        | 2,500                | 220MB             |
| Microservice  | 6        | 2,000                | 200MB             |
| Full          | 8        | 3,500                | 250MB             |

## Performance Considerations

### Startup Time Impact

- **Minimal**: < 2 seconds
- **Standard**: 3-5 seconds
- **Full**: 5-8 seconds (due to OpenTelemetry initialization)

### Memory Usage Impact

- **Minimal**: ~40MB
- **Standard**: ~80MB
- **Full**: ~120MB

### Build Time Impact

- **Minimal**: 10-15 seconds
- **Standard**: 30-45 seconds
- **Full**: 60-90 seconds (includes test compilation)

This guide helps you make informed decisions about feature selection based on your specific requirements and constraints.
