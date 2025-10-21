using Microsoft.AspNetCore.Builder;

namespace Enterprise.Ui.Security;

public static class SecurityHeaders
{
    public static IApplicationBuilder UseEnterpriseSecurityHeaders(this IApplicationBuilder app, bool dev = false)
    {
        return app.Use(async (ctx, next) =>
        {
            ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
            ctx.Response.Headers["X-Frame-Options"] = "DENY";
            ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            ctx.Response.Headers["X-XSS-Protection"] = "0";
            var csp = "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com; script-src 'self'; connect-src 'self' https://localhost:8080";
            if (dev) csp += " ws://localhost:*";
            ctx.Response.Headers["Content-Security-Policy"] = csp;
            await next();
        });
    }
}
