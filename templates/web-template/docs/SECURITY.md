# Security Guide

This document outlines the security features, patterns, and best practices implemented in the Enterprise UI Template.

## 🔒 Security Overview

The Enterprise UI Template implements defense-in-depth security principles with multiple layers of protection:

1. **Transport Security**: HTTPS enforcement and secure communications
2. **Authentication**: OpenID Connect (OIDC) integration
3. **Authorization**: Role-based access control (RBAC)
4. **Input Validation**: Anti-forgery tokens and request validation
5. **Output Security**: Content Security Policy and security headers
6. **Session Security**: Secure session management and token handling

## 🛡️ Security Headers

### Content Security Policy (CSP)

The application implements a restrictive Content Security Policy to prevent XSS attacks:

```csharp
public static class SecurityHeaders
{
    public static IApplicationBuilder UseEnterpriseSecurityHeaders(
        this IApplicationBuilder app, bool dev = false)
    {
        return app.Use(async (ctx, next) =>
        {
            // Security headers configuration
            ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
            ctx.Response.Headers["X-Frame-Options"] = "DENY";
            ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            ctx.Response.Headers["X-XSS-Protection"] = "0"; // Modern browsers handle XSS protection

            // Content Security Policy
            var csp = "default-src 'self'; " +
                     "img-src 'self' data:; " +
                     "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                     "font-src 'self' https://fonts.gstatic.com; " +
                     "script-src 'self'; " +
                     "connect-src 'self' https://localhost:8080";

            if (dev) csp += " ws://localhost:*"; // WebSocket for development

            ctx.Response.Headers["Content-Security-Policy"] = csp;
            await next();
        });
    }
}
```

### Security Header Breakdown

| Header                    | Value                             | Purpose                                                    |
| ------------------------- | --------------------------------- | ---------------------------------------------------------- |
| `X-Content-Type-Options`  | `nosniff`                         | Prevents MIME type sniffing attacks                        |
| `X-Frame-Options`         | `DENY`                            | Prevents clickjacking by blocking iframe embedding         |
| `Referrer-Policy`         | `strict-origin-when-cross-origin` | Controls referrer information leakage                      |
| `X-XSS-Protection`        | `0`                               | Disabled (modern browsers have better built-in protection) |
| `Content-Security-Policy` | Custom policy                     | Prevents XSS by controlling resource loading               |

### CSP Directives Explained

- **`default-src 'self'`**: Only allow resources from the same origin by default
- **`img-src 'self' data:`**: Images from same origin + data URLs (for inline images)
- **`style-src 'self' 'unsafe-inline' https://fonts.googleapis.com`**: Styles from same origin, inline styles, and Google Fonts
- **`font-src 'self' https://fonts.gstatic.com`**: Fonts from same origin and Google Fonts CDN
- **`script-src 'self'`**: Scripts only from same origin (no inline scripts)
- **`connect-src 'self' https://localhost:8080`**: API connections to same origin and local API

## 🔐 Authentication & Authorization

### OpenID Connect (OIDC) Integration

The template uses OIDC for modern, standards-based authentication:

```csharp
public static class AuthExtensions
{
    public static IServiceCollection AddEnterpriseAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authSection = configuration.GetSection("Auth");

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = authSection["Authority"];
            options.ClientId = authSection["ClientId"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            // Configure scopes
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add(authSection["ApiScope"] ?? "api");
        });

        services.AddAuthorization();
        return services;
    }
}
```

### Configuration Example

```json
{
  "Auth": {
    "Authority": "https://your-identity-provider.com/",
    "ClientId": "enterprise-ui-client",
    "ApiScope": "enterprise-api"
  }
}
```

### Role-Based Authorization

Implement role-based access control in components:

```csharp
@attribute [Authorize(Roles = "Admin,Manager")]

<AuthorizeView Roles="Admin">
    <Authorized>
        <AdminPanel />
    </Authorized>
    <NotAuthorized>
        <p>Access denied. Admin role required.</p>
    </NotAuthorized>
</AuthorizeView>
```

### Token Management

Tokens are automatically managed by the OIDC middleware:

- **Access Tokens**: Automatically refreshed and forwarded to APIs
- **Refresh Tokens**: Used for seamless token renewal
- **ID Tokens**: Used for user identity and claims

## 🔒 Input Validation & CSRF Protection

### Anti-Forgery Tokens

CSRF protection is enabled application-wide:

```csharp
// Program.cs
builder.Services.AddAntiforgery();

// Middleware pipeline
app.UseAntiforgery();
```

### Form Protection

Blazor forms are automatically protected:

```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <AntiforgeryToken />
    <!-- Form fields -->
    <button type="submit">Submit</button>
</EditForm>
```

### Input Validation

Use Data Annotations for server-side validation:

```csharp
public class UserModel
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]*$")]
    public string Username { get; set; } = "";
}
```

## 🌐 HTTPS and Transport Security

### HTTPS Enforcement

HTTPS is enforced in all environments:

```csharp
// Program.cs
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
```

### Certificate Configuration

**Development**:

```bash
dotnet dev-certs https --trust
```

**Production**:

- Use certificates from trusted Certificate Authority
- Configure certificate in hosting environment (IIS, Kestrel, etc.)
- Consider Let's Encrypt for automated certificate management

### Secure Cookie Configuration

```csharp
services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always; // HTTPS only
    options.HttpOnly = HttpOnlyPolicy.Always;   // No JavaScript access
});
```

## 🔐 Secrets Management

### Development Secrets

Use User Secrets for development:

```bash
dotnet user-secrets init
dotnet user-secrets set "Auth:ClientSecret" "your-secret-here"
```

### Production Secrets

**Azure Key Vault Integration**:

```csharp
// Program.cs
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        vaultUrl: "https://your-keyvault.vault.azure.net/",
        credential: new DefaultAzureCredential());
}
```

**Environment Variables**:

```bash
export Auth__ClientSecret="production-secret"
export ConnectionStrings__Database="production-connection-string"
```

## 🛡️ Error Handling Security

### Secure Error Messages

Error boundaries prevent sensitive information leakage:

```csharp
@inherits ErrorBoundaryBase

@if (CurrentException is null)
{
    @ChildContent
}
else
{
    <MudAlert Severity="Severity.Error">
        @if (Environment.IsDevelopment())
        {
            <pre>@CurrentException.ToString()</pre>
        }
        else
        {
            <p>An error occurred. Please try again or contact support.</p>
        }
    </MudAlert>
}

@code {
    protected override Task OnErrorAsync(Exception exception)
    {
        // Log exception securely (without sensitive data)
        Logger.LogError(exception, "Component error occurred");
        return Task.CompletedTask;
    }
}
```

### Logging Security

Ensure sensitive data is not logged:

```csharp
public class SecureLoggingService
{
    private readonly ILogger<SecureLoggingService> _logger;

    public void LogUserAction(string userId, string action)
    {
        // ✅ Good: Log user ID (non-sensitive identifier)
        _logger.LogInformation("User {UserId} performed action {Action}",
            userId, action);
    }

    public void LogError(Exception ex, string context)
    {
        // ✅ Good: Log exception without sensitive data
        _logger.LogError(ex, "Error in context {Context}", context);

        // ❌ Bad: Don't log sensitive information
        // _logger.LogError("Password validation failed for {Password}", password);
    }
}
```

## 🔍 Security Monitoring

### Health Checks for Security

Monitor security-related components:

```csharp
public class SecurityHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Check certificate expiration
        // Verify HTTPS configuration
        // Check authentication service availability

        return Task.FromResult(HealthCheckResult.Healthy("Security checks passed"));
    }
}
```

### Audit Logging

Implement audit trails for security events:

```csharp
public class AuditService
{
    private readonly ILogger<AuditService> _logger;

    public void LogSecurityEvent(string eventType, string userId,
        string details, bool isSuccess)
    {
        _logger.LogWarning("Security Event: {EventType} | User: {UserId} | " +
            "Success: {IsSuccess} | Details: {Details}",
            eventType, userId, isSuccess, details);
    }
}

// Usage examples
auditService.LogSecurityEvent("Login", userId, "OIDC authentication", true);
auditService.LogSecurityEvent("Unauthorized Access", userId, "Admin page access denied", false);
```

## 🚨 Security Best Practices

### Development Best Practices

1. **Never commit secrets to source control**

   ```bash
   # Use .gitignore
   appsettings.*.json
   *.Development.json
   .env
   ```

2. **Use HTTPS everywhere in production**

   ```csharp
   if (!builder.Environment.IsDevelopment())
   {
       builder.Services.AddHsts(options =>
       {
           options.Preload = true;
           options.IncludeSubDomains = true;
           options.MaxAge = TimeSpan.FromDays(365);
       });
   }
   ```

3. **Implement proper session timeout**
   ```csharp
   services.ConfigureApplicationCookie(options =>
   {
       options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
       options.SlidingExpiration = true;
   });
   ```

### Deployment Security

1. **Security Headers in Production**

   ```csharp
   if (app.Environment.IsProduction())
   {
       app.UseHsts(); // HTTP Strict Transport Security
       app.UseHttpsRedirection();
   }
   ```

2. **Remove Sensitive Information**

   ```csharp
   // Don't expose server information
   app.Use(async (context, next) =>
   {
       context.Response.Headers.Remove("Server");
       context.Response.Headers.Remove("X-Powered-By");
       await next();
   });
   ```

3. **Configure Reverse Proxy Security**
   ```nginx
   # nginx example
   add_header X-Frame-Options DENY;
   add_header X-Content-Type-Options nosniff;
   add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
   ```

## 🔐 Compliance Considerations

### GDPR Compliance

1. **Data Minimization**: Only collect necessary user data
2. **Consent Management**: Implement cookie consent banners
3. **Data Portability**: Provide user data export functionality
4. **Right to be Forgotten**: Implement user data deletion

### OWASP Top 10 Mitigation

| Vulnerability                        | Mitigation                                         |
| ------------------------------------ | -------------------------------------------------- |
| **A01: Broken Access Control**       | Role-based authorization, AuthorizeView components |
| **A02: Cryptographic Failures**      | HTTPS enforcement, secure cookie settings          |
| **A03: Injection**                   | Parameterized queries, input validation            |
| **A04: Insecure Design**             | Security-by-design architecture patterns           |
| **A05: Security Misconfiguration**   | Secure defaults, security headers                  |
| **A06: Vulnerable Components**       | Regular dependency updates, vulnerability scanning |
| **A07: Authentication Failures**     | OIDC standard implementation, session management   |
| **A08: Software Integrity Failures** | Dependency integrity checks, CSP                   |
| **A09: Logging Failures**            | Comprehensive audit logging, security monitoring   |
| **A10: SSRF**                        | Restricted HTTP client configurations              |

## 🔧 Security Testing

### Security Test Examples

```csharp
[Test]
public async Task Should_RedirectToHttps()
{
    var response = await _client.GetAsync("http://localhost/");
    response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    response.Headers.Location.Scheme.Should().Be("https");
}

[Test]
public async Task Should_SetSecurityHeaders()
{
    var response = await _client.GetAsync("/");
    response.Headers.Should().ContainKey("X-Content-Type-Options");
    response.Headers.Should().ContainKey("X-Frame-Options");
    response.Headers.Should().ContainKey("Content-Security-Policy");
}

[Test]
public async Task Should_RequireAuthenticationForProtectedPages()
{
    var response = await _client.GetAsync("/admin");
    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
}
```

### Penetration Testing

Consider regular security assessments:

1. **Automated Vulnerability Scanning**: Tools like OWASP ZAP, Burp Suite
2. **Static Code Analysis**: SonarQube, CodeQL
3. **Dependency Scanning**: WhiteSource, Snyk
4. **Professional Penetration Testing**: Annual third-party assessments

This security guide provides a comprehensive foundation for building secure enterprise applications with proper authentication, authorization, and defense-in-depth security measures.
