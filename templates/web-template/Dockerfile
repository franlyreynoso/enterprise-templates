# Enterprise Blazor UI Template - Production Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files for dependency restoration
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Enterprise.App.sln", "./"]

# Copy all source projects
COPY ["src/Enterprise.App/Enterprise.App.csproj", "src/Enterprise.App/"]
COPY ["src/Enterprise.Ui.Shared/Enterprise.Ui.Shared.csproj", "src/Enterprise.Ui.Shared/"]
#if (EnableAuth)
COPY ["src/Enterprise.Ui.Auth/Enterprise.Ui.Auth.csproj", "src/Enterprise.Ui.Auth/"]
#endif
#if (EnableHttpResilience)
COPY ["src/Enterprise.Ui.Http/Enterprise.Ui.Http.csproj", "src/Enterprise.Ui.Http/"]
#endif
#if (EnableApiClient)
COPY ["src/Enterprise.Ui.ApiClient/Enterprise.Ui.ApiClient.csproj", "src/Enterprise.Ui.ApiClient/"]
#endif
#if (EnableObservability)
COPY ["src/Enterprise.Ui.Observability/Enterprise.Ui.Observability.csproj", "src/Enterprise.Ui.Observability/"]
#endif
#if (EnableFeatureFlags)
COPY ["src/Enterprise.Ui.FeatureFlags/Enterprise.Ui.FeatureFlags.csproj", "src/Enterprise.Ui.FeatureFlags/"]
#endif
#if (EnableSecurity)
COPY ["src/Enterprise.Ui.Security/Enterprise.Ui.Security.csproj", "src/Enterprise.Ui.Security/"]
#endif

# Restore dependencies
RUN dotnet restore "src/Enterprise.App/Enterprise.App.csproj"

# Copy all source code
COPY ["src/", "src/"]

# Build application
RUN dotnet build "src/Enterprise.App/Enterprise.App.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "src/Enterprise.App/Enterprise.App.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Enterprise.App.dll"]