# Testing Guide

This document covers the comprehensive testing strategy implemented in the Enterprise UI Template, including unit testing with bUnit, integration testing, and end-to-end testing with Playwright.

## 🧪 Testing Strategy Overview

The Enterprise UI Template implements a multi-layered testing approach:

1. **Unit Tests**: Fast, isolated tests for individual components and services
2. **Integration Tests**: Tests that verify component interactions and API integration
3. **End-to-End Tests**: Full application workflow testing with Playwright
4. **Component Tests**: Blazor component testing with bUnit
5. **Performance Tests**: Load testing and performance validation

## 📋 Test Project Structure

```
tests/
├── Enterprise.Ui.Tests/
│   ├── Enterprise.Ui.Tests.csproj
│   ├── Components/
│   │   ├── LayoutTests.cs
│   │   ├── NavigationTests.cs
│   │   └── ThemeTests.cs
│   ├── Services/
│   │   ├── AuthServiceTests.cs
│   │   ├── HttpServiceTests.cs
│   │   └── HealthServiceTests.cs
│   ├── Integration/
│   │   ├── ApiIntegrationTests.cs
│   │   └── AuthenticationIntegrationTests.cs
│   ├── E2E/
│   │   ├── LoginFlowTests.cs
│   │   ├── NavigationFlowTests.cs
│   │   └── ResponsiveDesignTests.cs
│   └── TestHelpers/
│       ├── TestContext.cs
│       ├── MockServices.cs
│       └── TestData.cs
```

## 🔬 Unit Testing with bUnit

### Test Project Configuration

```xml
<!-- Enterprise.Ui.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="bunit" />
    <PackageReference Include="bunit.web" />
    <PackageReference Include="FluentAssertions" />
    <PackageReference Include="NSubstitute" />
    <PackageReference Include="MSTest.TestAdapter" />
    <PackageReference Include="MSTest.TestFramework" />
    <PackageReference Include="coverlet.collector" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Enterprise.App\Enterprise.App.csproj" />
    <ProjectReference Include="..\..\src\Enterprise.Ui.Shared\Enterprise.Ui.Shared.csproj" />
  </ItemGroup>
</Project>
```

### Base Test Class

```csharp
// TestHelpers/TestContext.cs
public abstract class TestContextBase : TestContext, IDisposable
{
    protected TestContextBase()
    {
        // Configure services for testing
        Services.AddLogging();
        Services.AddSingleton<IStringLocalizer>(NSubstitute.Substitute.For<IStringLocalizer>());

        // Add MudBlazor services
        Services.AddMudServices();

        // Add authentication testing
        this.AddTestAuthorization();

        // Configure mock HTTP client
        var mockHttpClient = Substitute.For<HttpClient>();
        Services.AddSingleton(mockHttpClient);
    }

    protected IRenderedComponent<T> RenderComponent<T>(params ComponentParameter[] parameters)
        where T : IComponent
    {
        return base.RenderComponent<T>(parameters);
    }

    public new void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
```

### Component Testing Examples

#### Layout Component Testing

```csharp
// Components/LayoutTests.cs
[TestClass]
public class LayoutTests : TestContextBase
{
    [TestMethod]
    public void MainLayout_ShouldRender_WithNavigationMenu()
    {
        // Arrange & Act
        var component = RenderComponent<MainLayout>();

        // Assert
        component.Should().NotBeNull();
        component.Find("nav").Should().NotBeNull();
        component.Find(".mud-drawer").Should().NotBeNull();
    }

    [TestMethod]
    public void MainLayout_ShouldToggle_DrawerVisibility()
    {
        // Arrange
        var component = RenderComponent<MainLayout>();
        var toggleButton = component.Find("[data-testid='drawer-toggle']");

        // Act
        toggleButton.Click();

        // Assert
        var drawer = component.Find(".mud-drawer");
        drawer.ClassList.Should().Contain("mud-drawer--closed");
    }

    [TestMethod]
    public void MainLayout_ShouldDisplay_UserInformation_WhenAuthenticated()
    {
        // Arrange
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("test-user");
        authContext.SetClaims(
            new Claim(ClaimTypes.Name, "John Doe"),
            new Claim(ClaimTypes.Email, "john.doe@example.com")
        );

        // Act
        var component = RenderComponent<MainLayout>();

        // Assert
        component.Find("[data-testid='user-info']").TextContent
            .Should().Contain("John Doe");
    }
}
```

#### Theme Component Testing

```csharp
// Components/ThemeTests.cs
[TestClass]
public class ThemeTests : TestContextBase
{
    [TestMethod]
    public void ThemeProvider_ShouldRender_WithDefaultTheme()
    {
        // Act
        var component = RenderComponent<ThemeProvider>();

        // Assert
        component.Should().NotBeNull();
        var mudThemeProvider = component.FindComponent<MudThemeProvider>();
        mudThemeProvider.Should().NotBeNull();
    }

    [TestMethod]
    public void ThemeToggle_ShouldSwitch_BetweenLightAndDark()
    {
        // Arrange
        var component = RenderComponent<ThemeProvider>();
        var toggleButton = component.Find("[data-testid='theme-toggle']");

        // Act - Switch to dark mode
        toggleButton.Click();

        // Assert
        var themeProvider = component.FindComponent<MudThemeProvider>();
        themeProvider.Instance.IsDarkMode.Should().BeTrue();

        // Act - Switch back to light mode
        toggleButton.Click();

        // Assert
        themeProvider.Instance.IsDarkMode.Should().BeFalse();
    }
}
```

#### Navigation Component Testing

```csharp
// Components/NavigationTests.cs
[TestClass]
public class NavigationTests : TestContextBase
{
    [TestMethod]
    public void NavMenu_ShouldRender_AllNavigationItems()
    {
        // Act
        var component = RenderComponent<NavMenu>();

        // Assert
        component.FindAll(".mud-nav-link").Should().HaveCountGreaterThan(0);
        component.Find("[href='/']").Should().NotBeNull(); // Home
        component.Find("[href='/health']").Should().NotBeNull(); // Health
    }

    [TestMethod]
    public void NavMenu_ShouldHighlight_ActiveRoute()
    {
        // Arrange
        var navigationManager = Services.GetRequiredService<NavigationManager>();
        navigationManager.NavigateTo("/health");

        // Act
        var component = RenderComponent<NavMenu>();

        // Assert
        var healthLink = component.Find("[href='/health']");
        healthLink.ClassList.Should().Contain("mud-nav-link-active");
    }

    [TestMethod]
    public void NavMenu_ShouldShow_AdminItems_ForAdminUsers()
    {
        // Arrange
        var authContext = this.AddTestAuthorization();
        authContext.SetAuthorized("admin-user");
        authContext.SetRoles("Admin");

        // Act
        var component = RenderComponent<NavMenu>();

        // Assert
        component.Find("[data-testid='admin-section']").Should().NotBeNull();
    }
}
```

### Service Testing Examples

#### HTTP Service Testing

```csharp
// Services/HttpServiceTests.cs
[TestClass]
public class HttpServiceTests
{
    private readonly HttpClient _mockHttpClient;
    private readonly IOptions<HttpClientOptions> _options;
    private readonly ILogger<ApiService> _logger;
    private readonly ApiService _service;

    public HttpServiceTests()
    {
        _mockHttpClient = Substitute.For<HttpClient>();
        _options = Substitute.For<IOptions<HttpClientOptions>>();
        _options.Value.Returns(new HttpClientOptions
        {
            ApiClient = new ApiClientOptions
            {
                BaseUrl = "https://api.test.com/",
                Timeout = TimeSpan.FromSeconds(30),
                RetryCount = 3
            }
        });
        _logger = Substitute.For<ILogger<ApiService>>();
        _service = new ApiService(_mockHttpClient, _options, _logger);
    }

    [TestMethod]
    public async Task GetAsync_ShouldReturn_DeserializedObject()
    {
        // Arrange
        var expectedData = new { Name = "Test", Value = 42 };
        var jsonResponse = JsonSerializer.Serialize(expectedData);

        _mockHttpClient.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.GetAsync<dynamic>("test-endpoint");

        // Assert
        result.Should().NotBeNull();
        // Additional assertions based on your specific implementation
    }

    [TestMethod]
    public async Task GetAsync_ShouldThrow_WhenApiReturnsError()
    {
        // Arrange
        _mockHttpClient.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.BadRequest));

        // Act & Assert
        await _service.Invoking(s => s.GetAsync<object>("test-endpoint"))
            .Should().ThrowAsync<HttpRequestException>();
    }
}
```

#### Health Service Testing

```csharp
// Services/HealthServiceTests.cs
[TestClass]
public class HealthServiceTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthService> _logger;
    private readonly HealthService _service;

    public HealthServiceTests()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _logger = Substitute.For<ILogger<HealthService>>();
        _service = new HealthService(_httpClientFactory, _logger);

        var mockHttpClient = Substitute.For<HttpClient>();
        _httpClientFactory.CreateClient().Returns(mockHttpClient);
    }

    [TestMethod]
    public async Task CheckApplicationHealthAsync_ShouldReturn_HealthyStatus()
    {
        // Act
        var result = await _service.CheckApplicationHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Healthy");
        result.Details.Should().ContainKey("timestamp");
        result.Details.Should().ContainKey("uptime");
    }

    [TestMethod]
    public async Task CheckExternalApiHealthAsync_ShouldReturn_ApiStatus()
    {
        // Arrange
        var mockHttpClient = Substitute.For<HttpClient>();
        mockHttpClient.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage(HttpStatusCode.OK));
        _httpClientFactory.CreateClient().Returns(mockHttpClient);

        // Act
        var result = await _service.CheckExternalApiHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Healthy");
    }
}
```

## 🌐 Integration Testing

### Integration Test Setup

```csharp
// Integration/IntegrationTestBase.cs
public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace services with test implementations
                services.RemoveAll<IAuthService>();
                services.AddScoped<IAuthService, MockAuthService>();

                services.RemoveAll<IEmailService>();
                services.AddScoped<IEmailService, MockEmailService>();
            });

            builder.UseEnvironment("Testing");
        }).CreateClient();
    }
}
```

### API Integration Tests

```csharp
// Integration/ApiIntegrationTests.cs
[TestClass]
public class ApiIntegrationTests : IntegrationTestBase
{
    public ApiIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [TestMethod]
    public async Task HealthEndpoint_ShouldReturn_HealthyStatus()
    {
        // Act
        var response = await Client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [TestMethod]
    public async Task HealthReadiness_ShouldReturn_ReadinessStatus()
    {
        // Act
        var response = await Client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var healthResult = await response.Content.ReadFromJsonAsync<HealthCheckResult>();
        healthResult?.Status.Should().Be("Healthy");
    }

    [TestMethod]
    public async Task SecureEndpoint_ShouldRequire_Authentication()
    {
        // Act
        var response = await Client.GetAsync("/admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location?.ToString().Should().Contain("login");
    }
}
```

### Authentication Integration Tests

```csharp
// Integration/AuthenticationIntegrationTests.cs
[TestClass]
public class AuthenticationIntegrationTests : IntegrationTestBase
{
    public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [TestMethod]
    public async Task Login_ShouldRedirect_ToIdentityProvider()
    {
        // Act
        var response = await Client.GetAsync("/login");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        var location = response.Headers.Location?.ToString();
        location.Should().Contain("oauth/authorize");
    }

    [TestMethod]
    public async Task AuthenticatedUser_ShouldAccess_ProtectedPages()
    {
        // Arrange
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "mock-token");

        // Act
        var response = await Client.GetAsync("/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## 🎭 End-to-End Testing with Playwright

### Playwright Setup

```csharp
// E2E/PlaywrightTestBase.cs
[TestClass]
public abstract class PlaywrightTestBase
{
    protected IPlaywright? Playwright { get; private set; }
    protected IBrowser? Browser { get; private set; }
    protected IBrowserContext? Context { get; private set; }
    protected IPage? Page { get; private set; }

    [TestInitialize]
    public async Task Setup()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true, // Set to false for debugging
            SlowMo = 50 // Slow down for better visibility during debugging
        });

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            Locale = "en-US"
        });

        Page = await Context.NewPageAsync();

        // Navigate to application
        await Page.GotoAsync("https://localhost:7000");
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await Page?.CloseAsync();
        await Context?.CloseAsync();
        await Browser?.CloseAsync();
        Playwright?.Dispose();
    }
}
```

### E2E Test Examples

#### Navigation Flow Tests

```csharp
// E2E/NavigationFlowTests.cs
[TestClass]
public class NavigationFlowTests : PlaywrightTestBase
{
    [TestMethod]
    public async Task ShouldNavigate_BetweenPages_Successfully()
    {
        // Verify home page loads
        await Page!.WaitForSelectorAsync("h1");
        var title = await Page.TextContentAsync("h1");
        title.Should().Contain("Enterprise UI Template");

        // Navigate to health page
        await Page.ClickAsync("a[href='/health']");
        await Page.WaitForSelectorAsync("[data-testid='health-page']");

        // Verify health page loaded
        var healthTitle = await Page.TextContentAsync("h2");
        healthTitle.Should().Contain("Application Health");

        // Test health check button
        await Page.ClickAsync("[data-testid='check-health-btn']");
        await Page.WaitForSelectorAsync("[data-testid='health-result']");

        var healthResult = await Page.TextContentAsync("[data-testid='health-result']");
        healthResult.Should().Contain("Healthy");
    }

    [TestMethod]
    public async Task ShouldToggle_ThemeMode_Successfully()
    {
        // Wait for theme toggle button
        await Page!.WaitForSelectorAsync("[data-testid='theme-toggle']");

        // Check initial theme (should be light)
        var body = await Page.QuerySelectorAsync("body");
        var initialClass = await body!.GetAttributeAsync("class");

        // Toggle to dark mode
        await Page.ClickAsync("[data-testid='theme-toggle']");
        await Task.Delay(500); // Wait for animation

        // Verify dark mode
        var darkClass = await body.GetAttributeAsync("class");
        darkClass.Should().Contain("dark");

        // Toggle back to light mode
        await Page.ClickAsync("[data-testid='theme-toggle']");
        await Task.Delay(500);

        // Verify light mode
        var lightClass = await body.GetAttributeAsync("class");
        lightClass.Should().NotContain("dark");
    }
}
```

#### Login Flow Tests

```csharp
// E2E/LoginFlowTests.cs
[TestClass]
public class LoginFlowTests : PlaywrightTestBase
{
    [TestMethod]
    public async Task ShouldRedirect_ToLogin_ForProtectedPages()
    {
        // Navigate to protected page
        await Page!.GotoAsync("https://localhost:7000/admin");

        // Should redirect to login
        await Page.WaitForURLAsync("**/login**");

        // Verify login elements are present
        await Page.WaitForSelectorAsync("[data-testid='login-button']");
        var loginButton = await Page.TextContentAsync("[data-testid='login-button']");
        loginButton.Should().Contain("Sign In");
    }

    [TestMethod]
    public async Task ShouldLogin_AndAccess_ProtectedContent()
    {
        // This test would require a test identity provider setup
        // Implementation depends on your specific OIDC provider

        // Navigate to login
        await Page!.ClickAsync("[data-testid='login-button']");

        // Fill in test credentials (adjust based on your identity provider)
        await Page.FillAsync("#username", "testuser@example.com");
        await Page.FillAsync("#password", "TestPassword123!");
        await Page.ClickAsync("#submit");

        // Wait for redirect back to application
        await Page.WaitForURLAsync("https://localhost:7000/**");

        // Verify authenticated state
        await Page.WaitForSelectorAsync("[data-testid='user-menu']");
        var userMenu = await Page.TextContentAsync("[data-testid='user-menu']");
        userMenu.Should().Contain("testuser");
    }
}
```

#### Responsive Design Tests

```csharp
// E2E/ResponsiveDesignTests.cs
[TestClass]
public class ResponsiveDesignTests : PlaywrightTestBase
{
    [TestMethod]
    public async Task ShouldAdapt_ToMobileViewport()
    {
        // Set mobile viewport
        await Page!.SetViewportSizeAsync(375, 667); // iPhone SE

        // Verify mobile navigation
        var drawer = await Page.QuerySelectorAsync(".mud-drawer");
        var isHidden = await drawer!.IsHiddenAsync();
        isHidden.Should().BeTrue(); // Drawer should be hidden on mobile

        // Open mobile menu
        await Page.ClickAsync("[data-testid='mobile-menu-toggle']");

        // Verify menu opens
        await Page.WaitForSelectorAsync(".mud-drawer--open");
        var drawerVisible = await Page.IsVisibleAsync(".mud-drawer--open");
        drawerVisible.Should().BeTrue();
    }

    [TestMethod]
    public async Task ShouldAdapt_ToTabletViewport()
    {
        // Set tablet viewport
        await Page!.SetViewportSizeAsync(768, 1024); // iPad

        // Verify tablet layout
        var mainContent = await Page.QuerySelectorAsync(".main-content");
        var width = await mainContent!.BoundingBoxAsync();
        width!.Width.Should().BeGreaterThan(700);
    }

    [TestMethod]
    public async Task ShouldAdapt_ToDesktopViewport()
    {
        // Set desktop viewport
        await Page!.SetViewportSizeAsync(1920, 1080);

        // Verify desktop navigation is always visible
        var drawer = await Page.QuerySelectorAsync(".mud-drawer");
        var isVisible = await drawer!.IsVisibleAsync();
        isVisible.Should().BeTrue();
    }
}
```

## 🚀 Performance Testing

### Load Testing with NBomber

```csharp
// Performance/LoadTests.cs
[TestClass]
public class LoadTests
{
    [TestMethod]
    public void HomePageLoad_ShouldHandle_ConcurrentUsers()
    {
        var scenario = Scenario.Create("home_page_load", async context =>
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7000");

            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(1))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }

    [TestMethod]
    public void ApiEndpoints_ShouldHandle_HighThroughput()
    {
        var scenario = Scenario.Create("api_load", async context =>
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7000/health");

            return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(2))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
}
```

## 🔧 Test Configuration & Setup

### Test Settings

```json
// testsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  },

  "Testing": {
    "UseMockServices": true,
    "DatabaseProvider": "InMemory",
    "IdentityProvider": "Mock"
  },

  "Playwright": {
    "Headless": true,
    "SlowMo": 0,
    "Timeout": 30000,
    "BaseUrl": "https://localhost:7000"
  }
}
```

### CI/CD Test Pipeline

```yaml
# .github/workflows/tests.yml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "9.0.x"

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Install Playwright
        run: |
          dotnet tool install --global Microsoft.Playwright.CLI
          playwright install

      - name: Run Unit Tests
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"

      - name: Start Application
        run: |
          dotnet run --project src/Enterprise.App/Enterprise.App.csproj &
          sleep 30 # Wait for app to start

      - name: Run E2E Tests
        run: dotnet test tests/Enterprise.Ui.Tests/Enterprise.Ui.Tests.csproj --filter "Category=E2E"

      - name: Upload Coverage Reports
        uses: codecov/codecov-action@v3
        with:
          files: ./coverage.xml
```

## 📊 Test Reporting & Coverage

### Code Coverage Configuration

```xml
<!-- Directory.Build.props -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>opencover</CoverletOutputFormat>
  <CoverletOutput>../coverage/</CoverletOutput>
  <ExcludeByFile>**/bin/**/*;**/obj/**/*</ExcludeByFile>
</PropertyGroup>
```

### Running Tests with Coverage

```bash
# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Generate coverage report
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

## 🎯 Testing Best Practices

### Unit Test Best Practices

1. **Follow AAA Pattern**: Arrange, Act, Assert
2. **Test One Thing**: Each test should verify one behavior
3. **Use Descriptive Names**: Test names should describe what's being tested
4. **Keep Tests Independent**: Tests shouldn't depend on each other
5. **Mock External Dependencies**: Isolate units being tested

### Integration Test Best Practices

1. **Test Real Integrations**: Use actual database/API connections when needed
2. **Use Test Doubles**: Mock external services you don't control
3. **Clean Up**: Ensure tests don't leave side effects
4. **Test Happy and Sad Paths**: Cover both success and failure scenarios

### E2E Test Best Practices

1. **Test User Journeys**: Focus on complete workflows
2. **Use Page Object Model**: Organize page interactions
3. **Wait for Elements**: Use proper waiting strategies
4. **Run in CI/CD**: Automate E2E tests in deployment pipeline
5. **Keep Tests Stable**: Avoid flaky tests with proper waits and selectors

### Performance Test Best Practices

1. **Test Realistic Scenarios**: Use production-like data and load
2. **Monitor Key Metrics**: Response time, throughput, resource usage
3. **Set Performance Budgets**: Define acceptable performance thresholds
4. **Run Regularly**: Include performance tests in CI/CD pipeline

This comprehensive testing guide provides the foundation for building reliable, well-tested enterprise applications with multiple testing strategies and best practices.
