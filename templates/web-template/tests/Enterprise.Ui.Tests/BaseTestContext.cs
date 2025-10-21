using Bunit;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Ui.Tests;

/// <summary>
/// Base test context class that provides proper setup for testing Blazor components.
/// This class can be extended for component testing scenarios that require service registration.
/// </summary>
public abstract class BaseTestContext : TestContext
{
    protected BaseTestContext()
    {
        // Register MudBlazor services for component testing
        Services.AddMudServices();

        // Add any other common services needed for testing
        // Example: Services.AddScoped<IMyService, MockMyService>();

        // Note: For testing complex MudBlazor components that require JSInterop,
        // additional JSInterop setup may be needed. Consider using integration tests
        // or E2E tests for components with complex JavaScript interactions.
    }
}