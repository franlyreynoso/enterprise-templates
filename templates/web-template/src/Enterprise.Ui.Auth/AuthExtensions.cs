using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Ui.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddEnterpriseAuth(
        this IServiceCollection services, IConfiguration cfg, string policyAdmin = "RequireAdmin")
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = cfg["Auth:Authority"] ?? "https://issuer.example.com/";
            options.ClientId = cfg["Auth:ClientId"] ?? "ui-client";
            options.ClientSecret = cfg["Auth:ClientSecret"];
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            var apiScope = cfg["Auth:ApiScope"];
            if (!string.IsNullOrWhiteSpace(apiScope)) options.Scope.Add(apiScope);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(policyAdmin, p => p.RequireRole("Admin", "SuperAdmin"));
        });

        services.AddCascadingAuthenticationState();
        return services;
    }
}
