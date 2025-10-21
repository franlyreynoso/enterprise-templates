# Development Guide

This document provides comprehensive guidance for developers working with the Enterprise UI Template, including development workflow, debugging techniques, local development setup, and contribution guidelines.

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** - Latest .NET SDK
- **[Visual Studio 2022](https://visualstudio.microsoft.com/vs/)** (17.8+) or **[VS Code](https://code.visualstudio.com/)**
- **[Git](https://git-scm.com/)** - Version control
- **[Node.js 18+](https://nodejs.org/)** - For frontend tooling (optional)
- **[Docker Desktop](https://www.docker.com/products/docker-desktop/)** - For containerization (optional)

### Development Environment Setup

1. **Clone the repository**:

   ```bash
   git clone https://github.com/your-org/enterprise-ui-template-full.git
   cd enterprise-ui-template-full
   ```

2. **Restore dependencies**:

   ```bash
   dotnet restore
   ```

3. **Set up user secrets** (for development configuration):

   ```bash
   dotnet user-secrets init --project src/Enterprise.App
   dotnet user-secrets set "Auth:ClientSecret" "your-dev-secret" --project src/Enterprise.App
   ```

4. **Build the solution**:

   ```bash
   dotnet build
   ```

5. **Run the application**:

   ```bash
   dotnet run --project src/Enterprise.App
   ```

6. **Open in browser**: Navigate to `https://localhost:7000`

## 💻 IDE Configuration

### Visual Studio 2022

#### Recommended Extensions

- **MudBlazor Extension** - Enhanced IntelliSense for MudBlazor components
- **OpenTelemetry .NET Toolkit** - Observability tooling
- **SonarLint** - Code quality analysis
- **EditorConfig** - Code formatting consistency

#### Launch Settings

The project includes multiple launch profiles in `launchSettings.json`:

```json
{
  "profiles": {
    "Enterprise.App": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7000;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Enterprise.App (Staging)": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7000;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Staging"
      }
    }
  }
}
```

### Visual Studio Code

#### Recommended Extensions

```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.csdevkit",
    "ms-dotnettools.blazorwasm-companion",
    "bradlc.vscode-tailwindcss",
    "esbenp.prettier-vscode",
    "ms-playwright.playwright",
    "sonarsource.sonarlint-vscode"
  ]
}
```

#### Debug Configuration (`.vscode/launch.json`)

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch Enterprise.App",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/src/Enterprise.App/bin/Debug/net9.0/Enterprise.App.dll",
      "args": [],
      "cwd": "${workspaceFolder}/src/Enterprise.App",
      "console": "internalConsole",
      "stopAtEntry": false,
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "https://localhost:7000;http://localhost:5000"
      }
    }
  ]
}
```

#### Build Tasks (`.vscode/tasks.json`)

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": ["build", "${workspaceFolder}/enterprise-ui-template-full.sln"],
      "group": "build",
      "presentation": {
        "reveal": "silent"
      },
      "problemMatcher": "$msCompile"
    },
    {
      "label": "test",
      "command": "dotnet",
      "type": "process",
      "args": [
        "test",
        "${workspaceFolder}/tests/Enterprise.Ui.Tests/Enterprise.Ui.Tests.csproj"
      ],
      "group": "test",
      "presentation": {
        "reveal": "always"
      },
      "problemMatcher": "$msCompile"
    }
  ]
}
```

## 🔧 Development Workflow

### Git Workflow

We follow **Git Flow** with the following branch structure:

- **`main`** - Production-ready code
- **`develop`** - Integration branch for features
- **`feature/*`** - Individual feature branches
- **`release/*`** - Release preparation branches
- **`hotfix/*`** - Critical production fixes

#### Feature Development Process

1. **Create feature branch**:

   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/your-feature-name
   ```

2. **Make changes and commit**:

   ```bash
   git add .
   git commit -m "feat: add new dashboard component"
   ```

3. **Push and create PR**:
   ```bash
   git push origin feature/your-feature-name
   # Create Pull Request to develop branch
   ```

### Commit Message Convention

We use **Conventional Commits** for consistency:

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

**Types**:

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style/formatting
- `refactor`: Code refactoring
- `test`: Adding/updating tests
- `chore`: Maintenance tasks

**Examples**:

```bash
git commit -m "feat(auth): add OIDC authentication support"
git commit -m "fix(ui): resolve mobile navigation menu issue"
git commit -m "docs: update API documentation"
```

## 🐛 Debugging

### Application Debugging

#### Enable Detailed Errors

In `appsettings.Development.json`:

```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.AspNetCore.SignalR": "Debug"
    }
  }
}
```

#### Blazor Server Debugging

1. **Browser Developer Tools**: Use F12 to inspect DOM and network requests
2. **Server-Side Debugging**: Set breakpoints in Razor components and C# code
3. **SignalR Debugging**: Monitor WebSocket connections in Network tab

#### Common Debugging Scenarios

**Component State Issues**:

```csharp
@code {
    protected override void OnAfterRender(bool firstRender)
    {
        // Add breakpoint here to inspect component state
        System.Diagnostics.Debugger.Break(); // Only in debug builds
        base.OnAfterRender(firstRender);
    }
}
```

**HTTP Client Issues**:

```csharp
public class ApiService
{
    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Log response for debugging
            _logger.LogDebug("API Response: {StatusCode} - {Content}",
                response.StatusCode, content);

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<T>(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for endpoint: {Endpoint}", endpoint);
            throw;
        }
    }
}
```

### Health Check Debugging

Access health check endpoints during development:

- **General Health**: `https://localhost:7000/health`
- **Detailed Health**: `https://localhost:7000/health/ready`
- **Live Health**: `https://localhost:7000/health/live`

### OpenTelemetry Debugging

Enable console exporter in development:

```json
{
  "Observability": {
    "EnableConsoleExporter": true
  }
}
```

## 🧪 Testing During Development

### Unit Tests

Run unit tests frequently during development:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Enterprise.Ui.Tests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Watch mode (re-run tests on file changes)
dotnet watch test --project tests/Enterprise.Ui.Tests/
```

### Component Testing with bUnit

```csharp
[TestMethod]
public void MyComponent_ShouldRender_Correctly()
{
    // Arrange
    var component = RenderComponent<MyComponent>(parameters => parameters
        .Add(p => p.Title, "Test Title")
        .Add(p => p.IsVisible, true));

    // Act & Assert
    component.Find("h1").TextContent.Should().Be("Test Title");
    component.Find(".visible").Should().NotBeNull();
}
```

### Integration Testing

```bash
# Run integration tests
dotnet test --filter "Category=Integration"

# Run with specific environment
ASPNETCORE_ENVIRONMENT=Testing dotnet test
```

### End-to-End Testing

```bash
# Install Playwright
dotnet tool install --global Microsoft.Playwright.CLI
playwright install

# Run E2E tests
dotnet test --filter "Category=E2E"
```

## 🔍 Performance Debugging

### Application Performance

#### Profiling with dotnet-trace

```bash
# Install profiling tools
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-counters

# Start application
dotnet run --project src/Enterprise.App &

# Collect performance trace
dotnet-trace collect --process-id <PID> --duration 00:00:30

# Monitor performance counters
dotnet-counters monitor --process-id <PID>
```

#### Memory Analysis

```bash
# Install memory analysis tools
dotnet tool install --global dotnet-dump
dotnet tool install --global dotnet-gcdump

# Collect memory dump
dotnet-dump collect --process-id <PID>

# Collect GC dump
dotnet-gcdump collect --process-id <PID>
```

### Blazor Server Performance

#### SignalR Connection Monitoring

```csharp
// Add to Program.cs for debugging
builder.Services.AddSignalR(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors = true;
        options.MaximumReceiveMessageSize = 128 * 1024; // 128KB
    }
});
```

#### Circuit Debugging

```csharp
builder.Services.AddServerSideBlazor(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.DetailedErrors = true;
        options.DisconnectedCircuitMaxRetained = 10;
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
    }
});
```

## 🔧 Hot Reload & Live Development

### .NET Hot Reload

```bash
# Start with hot reload enabled
dotnet watch run --project src/Enterprise.App

# Hot reload specific file types
dotnet watch run --non-interactive --property=DotNetWatchOptions=static
```

### CSS Hot Reload

The template includes CSS isolation and automatic reloading:

```html
<!-- Component.razor.css files are automatically reloaded -->
<div class="my-component">
  <h1>This will update on CSS changes</h1>
</div>
```

### Browser Sync (Optional)

For enhanced development experience:

```bash
npm install -g browser-sync
browser-sync start --proxy "localhost:7000" --files "**/*.css, **/*.js"
```

## 📦 Package Management

### Adding New NuGet Packages

With Central Package Management:

1. **Add package version to `Directory.Packages.props`**:

   ```xml
   <PackageVersion Include="NewPackage.Name" Version="1.0.0" />
   ```

2. **Reference in project file** (without version):
   ```xml
   <PackageReference Include="NewPackage.Name" />
   ```

### Updating Packages

```bash
# Update all packages
dotnet outdated --upgrade

# Update specific package
# Edit version in Directory.Packages.props, then:
dotnet restore
```

### Security Auditing

```bash
# Check for vulnerable packages
dotnet list package --vulnerable

# Update vulnerable packages
dotnet outdated --upgrade --include-prerelease
```

## 🐳 Containerization

### Docker Development

#### Dockerfile

```dockerfile
# Development Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["enterprise-ui-template-full.sln", "./"]
COPY ["src/", "src/"]

# Restore dependencies
RUN dotnet restore "src/Enterprise.App/Enterprise.App.csproj"

# Build application
RUN dotnet build "src/Enterprise.App/Enterprise.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Enterprise.App/Enterprise.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Enterprise.App.dll"]
```

#### Docker Compose for Development

```yaml
# docker-compose.dev.yml
version: "3.8"
services:
  app:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "7000:8080"
      - "7001:8081"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=https://+:8081;http://+:8080
    volumes:
      - ~/.aspnet/https:/https:ro
    depends_on:
      - db

  db:
    image: postgres:15
    environment:
      POSTGRES_DB: enterpriseui
      POSTGRES_USER: dev
      POSTGRES_PASSWORD: devpassword
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

#### Running with Docker

```bash
# Build and run
docker-compose -f docker-compose.dev.yml up --build

# Run in background
docker-compose -f docker-compose.dev.yml up -d

# View logs
docker-compose -f docker-compose.dev.yml logs -f app
```

## 🚀 Deployment

### Local Production Testing

```bash
# Build for production
dotnet publish -c Release -o ./publish

# Run production build locally
cd publish
dotnet Enterprise.App.dll --environment=Production
```

### Environment Variables

```bash
# Development
export ASPNETCORE_ENVIRONMENT=Development
export Auth__ClientSecret=dev-secret

# Staging
export ASPNETCORE_ENVIRONMENT=Staging
export Auth__ClientSecret=staging-secret

# Production
export ASPNETCORE_ENVIRONMENT=Production
export Auth__ClientSecret=production-secret
```

## 🤝 Contributing Guidelines

### Code Standards

#### C# Coding Standards

- Follow Microsoft C# coding conventions
- Use `var` when type is obvious
- Use meaningful variable names
- Add XML documentation for public APIs
- Use nullable reference types

#### Razor Component Standards

```razor
@* Good component structure *@
@page "/example"
@using Enterprise.Ui.Shared.Components
@inject ILogger<ExampleComponent> Logger

<PageTitle>Example Page</PageTitle>

<MudContainer MaxWidth="MaxWidth.Large">
    <MudText Typo="Typo.h3" GutterBottom="true">
        Example Component
    </MudText>

    @if (IsLoading)
    {
        <MudProgressCircular Indeterminate="true" />
    }
    else
    {
        <MudCard>
            <MudCardContent>
                <!-- Component content -->
            </MudCardContent>
        </MudCard>
    }
</MudContainer>

@code {
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnTitleChanged { get; set; }

    private bool IsLoading { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load data for {Component}", nameof(ExampleComponent));
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadDataAsync()
    {
        // Load data logic
        await Task.Delay(1000); // Simulate async operation
    }
}
```

### Pull Request Process

1. **Create feature branch** from `develop`
2. **Implement changes** following coding standards
3. **Write/update tests** for new functionality
4. **Update documentation** if needed
5. **Run full test suite** locally
6. **Create Pull Request** with clear description
7. **Address review feedback**
8. **Squash and merge** after approval

### PR Template

```markdown
## Description

Brief description of changes

## Type of Change

- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing

- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] E2E tests pass
- [ ] Manual testing completed

## Checklist

- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
```

### Code Review Guidelines

#### What Reviewers Should Check

- **Functionality**: Does the code work as intended?
- **Design**: Is the code well-designed and fit for the system?
- **Complexity**: Is the code more complex than necessary?
- **Tests**: Are there appropriate tests?
- **Naming**: Are variables, methods, and classes named clearly?
- **Comments**: Are comments clear and useful?
- **Documentation**: Is relevant documentation updated?

#### What Authors Should Do

- **Keep PRs small**: Easier to review and less likely to introduce bugs
- **Write clear descriptions**: Explain what and why, not just how
- **Self-review first**: Review your own code before requesting review
- **Respond promptly**: Address feedback quickly
- **Test thoroughly**: Ensure all tests pass

## 🛠️ Troubleshooting Common Issues

### Compilation Errors

**Missing Package References**:

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

**Central Package Management Issues**:

- Ensure `ManagePackageVersionsCentrally` is `true` in `Directory.Packages.props`
- Remove version numbers from `PackageReference` elements in `.csproj` files
- Add versions to `PackageVersion` elements in `Directory.Packages.props`

### Runtime Errors

**Blazor Circuit Disconnected**:

- Check browser console for JavaScript errors
- Verify SignalR connection is not blocked by proxy/firewall
- Check for memory issues or long-running operations blocking UI thread

**Authentication Issues**:

- Verify OIDC configuration in `appsettings.json`
- Check that identity provider is accessible
- Ensure redirect URIs are correctly configured

### Performance Issues

**Slow Page Loads**:

- Enable detailed timing in browser dev tools
- Check for N+1 query problems in data access
- Profile server-side rendering with dotnet-trace

**High Memory Usage**:

- Monitor with dotnet-counters
- Check for memory leaks in long-running operations
- Use dotnet-gcdump to analyze GC behavior

This development guide provides a comprehensive foundation for productive development with the Enterprise UI Template, covering everything from initial setup to contribution guidelines and troubleshooting common issues.
