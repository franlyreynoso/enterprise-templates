using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using EnterpriseTemplate.Application;
using EnterpriseTemplate.Application.Abstractions;
using EnterpriseTemplate.Application.Behaviors;
using EnterpriseTemplate.Application.Todos;
using FluentValidation;
using MassTransit;
using MediatR;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using EnterpriseTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Serilog (console)
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// OpenTelemetry (minimal)
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter());

// Authentication (OIDC/JWT)
var authMode = builder.Configuration["Auth:Mode"] ?? "Jwt";
var isDev = builder.Environment.IsDevelopment();

if (string.Equals(authMode, "Jwt", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.Authority = builder.Configuration["Auth:Authority"];
            o.Audience = builder.Configuration["Auth:Audience"];
            o.RequireHttpsMetadata = !isDev; // allow http only in Dev
        });

    builder.Services.AddAuthorization(opts =>
        {
            opts.AddPolicy("Todos.Read", p => p.RequireAssertion(_ => true));
            opts.AddPolicy("Todos.Create", p => p.RequireAssertion(_ => true));
        });
}
else
{
    // No authentication but still need authorization services for pipeline
    builder.Services.AddAuthorization();
}

// API bits
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnterpriseTemplate API", Version = "v1" });
});

// HTTP Client for testing
builder.Services.AddHttpClient();

// CQRS (MediatR)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationMarker).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(typeof(ApplicationMarker).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
builder.Services.AddSingleton<ITodoStore, InMemoryTodoStore>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Rate limiting
builder.Services.AddRateLimiter(o =>
{
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetTokenBucketLimiter("global", _ => new TokenBucketRateLimiterOptions
        {
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 20,
            TokenLimit = 40,
            QueueLimit = 0
        }));
});

// Infra packs (switchable)
var cloud = builder.Configuration["Cloud"] ?? "OnPrem";
if (string.Equals(cloud, "Azure", StringComparison.OrdinalIgnoreCase))
{
    EnterpriseTemplate.Infrastructure.AzurePack.ServiceCollectionExtensions.AddAzurePack(builder.Services, builder.Configuration);
}
else
{
    EnterpriseTemplate.Infrastructure.OnPremPack.ServiceCollectionExtensions.AddOnPremPack(builder.Services, builder.Configuration);
}

// Note: Health checks are configured in the infrastructure packs

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Use detailed error pages in dev
}
else
{
    // Global exception → ProblemDetails (RFC7807) for production
    app.UseExceptionHandler(errApp =>
    {
        errApp.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var ex = feature?.Error;

            ProblemDetails problem;
            int status;

            switch (ex)
            {
                case ValidationException vex:
                    status = (int)HttpStatusCode.BadRequest;
                    problem = new ProblemDetails
                    {
                        Status = status,
                        Title = "Validation failed",
                        Detail = "One or more validation errors occurred.",
                        Type = "https://tools.ietf.org/html/rfc7807"
                    };
                    problem.Extensions["errors"] = vex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                    break;

                case UnauthorizedAccessException:
                    status = (int)HttpStatusCode.Forbidden;
                    problem = new ProblemDetails
                    {
                        Status = status,
                        Title = "Forbidden",
                        Detail = "You do not have permission to perform this action.",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                    };
                    break;

                default:
                    status = (int)HttpStatusCode.InternalServerError;
                    problem = new ProblemDetails
                    {
                        Status = status,
                        Title = "Internal Server Error",
                        Detail = "An unexpected error occurred.",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                    };
                    break;
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        });
    });
}


app.UseSerilogRequestLogging();
app.UseRateLimiter();

app.UseXContentTypeOptions();
app.UseReferrerPolicy(x => x.NoReferrer());
app.UseXXssProtection(x => x.EnabledWithBlockMode());
app.UseXfo(x => x.Deny()); // deny iframing

// HTTPS redirection is commented out as HTTPS termination should be handled by reverse proxy
// If running in production with HTTPS configured, uncomment the following line:
// app.UseHttpsRedirection();

// Only use authentication middleware if authentication is configured
if (string.Equals(authMode, "Jwt", StringComparison.OrdinalIgnoreCase))
{
    app.UseAuthentication();
}

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enhanced health check endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false, // Exclude all checks for liveness (just returns if app is running)
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"), // Only include "ready" tagged checks
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/health/external", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("external"), // Only external service checks
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthCheckResponse // All health checks with detailed response
});

static async Task WriteHealthCheckResponse(HttpContext context, HealthReport healthReport)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var options = new JsonWriterOptions { Indented = true };
    using var memoryStream = new MemoryStream();
    using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", healthReport.Status.ToString());
        jsonWriter.WriteString("totalDuration", healthReport.TotalDuration.ToString());
        jsonWriter.WriteStartObject("results");

        foreach (var healthReportEntry in healthReport.Entries)
        {
            jsonWriter.WriteStartObject(healthReportEntry.Key);
            jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
            jsonWriter.WriteString("description", healthReportEntry.Value.Description);
            jsonWriter.WriteString("duration", healthReportEntry.Value.Duration.ToString());

            if (healthReportEntry.Value.Exception != null)
            {
                jsonWriter.WriteString("exception", healthReportEntry.Value.Exception.Message);
            }

            if (healthReportEntry.Value.Data.Any())
            {
                jsonWriter.WriteStartObject("data");
                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);
                    JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                }
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }

    await context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
}


// sample endpoints
app.MapGet("/todos", async (IMediator mediator) => Results.Ok(await mediator.Send(new ListMyTodos())));
app.MapPost("/todos", async (IMediator mediator, CreateTodo cmd) => Results.Ok(await mediator.Send(cmd)));

app.MapPost("/bus/ping", async (IPublishEndpoint bus) =>
{
    await bus.Publish(new EnterpriseTemplate.Application.Messaging.PingMessage(
        Text: "Hello from API",
        Timestamp: DateTime.UtcNow));
    return Results.Accepted();
});

// Test endpoint for HTTP client tracing with resilience
app.MapGet("/test/external", async (HttpClient httpClient, EnterpriseTemplate.Infrastructure.Resilience.IResilienceService resilienceService) =>
{
    try
    {
        // Use resilience service for external call with retry policy
        var response = await resilienceService.ExecuteAsync(async (ct) =>
        {
            var httpResponse = await httpClient.GetAsync("https://httpbin.org/delay/1", ct);
            return await httpResponse.Content.ReadAsStringAsync(ct);
        }, "external-api-call");

        return Results.Ok(new
        {
            message = "External call completed with resilience",
            timestamp = DateTime.UtcNow,
            response = response.Length > 100 ? $"{response[..100]}..." : response
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"External call failed after resilience policies: {ex.Message}");
    }
});

// Test endpoint to demonstrate circuit breaker behavior (deliberate failures)
app.MapGet("/test/external/unreliable", async (HttpClient httpClient, EnterpriseTemplate.Infrastructure.Resilience.IResilienceService resilienceService) =>
{
    try
    {
        // This endpoint will sometimes fail to demonstrate circuit breaker
        var random = new Random();
        var shouldFail = random.NextDouble() < 0.7; // 70% failure rate

        var response = await resilienceService.ExecuteAsync(async (ct) =>
        {
            var url = shouldFail ? "https://httpbin.org/status/500" : "https://httpbin.org/status/200";
            var httpResponse = await httpClient.GetAsync(url, ct);

            if (!httpResponse.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {httpResponse.StatusCode}");

            return $"Success: {httpResponse.StatusCode}";
        }, "unreliable-external-api");

        return Results.Ok(new
        {
            message = "Unreliable external call succeeded",
            timestamp = DateTime.UtcNow,
            response = response
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Unreliable external call failed: {ex.Message}");
    }
});

// Audit endpoints
app.MapGet("/audit/entity/{entityType}/{entityId}", async (IMediator mediator, string entityType, string entityId, int pageSize = 20, int pageNumber = 1) =>
{
    var query = new EnterpriseTemplate.Application.Auditing.GetEntityAuditTrail(entityType, entityId, pageSize, pageNumber);
    var result = await mediator.Send(query);
    return Results.Ok(result);
}).WithName("GetEntityAuditTrail").WithOpenApi();

app.MapGet("/audit/user/{userId}", async (IMediator mediator, string userId, int pageSize = 20, int pageNumber = 1, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null) =>
{
    var query = new EnterpriseTemplate.Application.Auditing.GetUserAuditTrail(userId, pageSize, pageNumber, fromDate, toDate);
    var result = await mediator.Send(query);
    return Results.Ok(result);
}).WithName("GetUserAuditTrail").WithOpenApi();

app.MapGet("/audit/recent", async (IMediator mediator, int pageSize = 20, int pageNumber = 1, string? entityType = null, string? action = null) =>
{
    var query = new EnterpriseTemplate.Application.Auditing.GetRecentAuditActivities(pageSize, pageNumber, entityType, action);
    var result = await mediator.Send(query);
    return Results.Ok(result);
}).WithName("GetRecentAuditActivities").WithOpenApi();

// Map controllers
app.MapControllers();

// Apply database migrations with retry logic
await ApplyDatabaseMigrationsAsync(app.Services);

app.Run();

static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services)
{
    const int maxRetries = 10;
    const int delaySeconds = 5;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("Attempting to apply database migrations (attempt {Attempt}/{MaxRetries})...", attempt, maxRetries);
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
            return;
        }
        catch (Exception ex)
        {
            if (attempt == maxRetries)
            {
                using var scope = services.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Failed to apply database migrations after {MaxRetries} attempts. Please ensure the database is accessible and configured correctly.", maxRetries);
                throw new InvalidOperationException($"Failed to connect to the database after {maxRetries} attempts. Please ensure PostgreSQL is running and the connection string is correct. See inner exception for details.", ex);
            }
            
            using (var scope = services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(ex, "Database migration attempt {Attempt}/{MaxRetries} failed. Retrying in {DelaySeconds} seconds...", attempt, maxRetries, delaySeconds);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

public partial class Program { }

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _ctx;
    public CurrentUser(IHttpContextAccessor ctx) => _ctx = ctx;
    public string? Id => _ctx.HttpContext?.User?.FindFirst("sub")?.Value ?? "dev-user";
    public bool Has(string permission) => true; // Demo: allow; replace with real policy/claims mapping
}

