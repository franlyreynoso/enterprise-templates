using MudBlazor.Services;
<!--#if (EnableAuth)-->
using Enterprise.Ui.Auth;
<!--#endif-->
<!--#if (EnableHttpResilience)-->
using Enterprise.Ui.Http;
<!--#endif-->
<!--#if (EnableObservability)-->
using Enterprise.Ui.Observability;
<!--#endif-->
<!--#if (EnableFeatureFlags)-->
using Enterprise.Ui.FeatureFlags;
<!--#endif-->
<!--#if (EnableSecurity)-->
using Enterprise.Ui.Security;
<!--#endif-->
using Enterprise.App.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

<!--#if (EnableAuth)-->
// Add authentication services
builder.Services.AddEnterpriseAuth(builder.Configuration);
<!--#endif-->

<!--#if (EnableHttpResilience)-->
// Add HTTP resilience services
builder.Services.AddEnterpriseHttpClient(builder.Configuration);
<!--#endif-->

<!--#if (EnableObservability)-->
// Add observability services
builder.Services.AddEnterpriseOtel(builder.Configuration, "Enterprise.App");
<!--#endif-->

<!--#if (EnableFeatureFlags)-->
// Add feature flags services
builder.Services.AddSingleton<Enterprise.Ui.FeatureFlags.IFeatureFlagProvider, Enterprise.Ui.FeatureFlags.ConfigFeatureFlags>();
<!--#endif-->

<!--#if (EnableSecurity)-->
// Add security services
builder.Services.AddAntiforgery();
<!--#endif-->

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

<!--#if (EnableAuth)-->
app.UseAuthentication();
app.UseAuthorization();
<!--#endif-->

<!--#if (EnableSecurity)-->
// Apply security headers
app.UseEnterpriseSecurityHeaders(!app.Environment.IsDevelopment());
<!--#endif-->

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
