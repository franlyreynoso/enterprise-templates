# Enterprise Clean Architecture API Template - Production Dockerfile
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
COPY ["EnterpriseTemplate.sln", "./"]

# Copy all source projects
COPY ["src/Api/EnterpriseTemplate.Api.csproj", "src/Api/"]
COPY ["src/Application/EnterpriseTemplate.Application.csproj", "src/Application/"]
COPY ["src/Domain/EnterpriseTemplate.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/EnterpriseTemplate.Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Worker/EnterpriseTemplate.Worker.csproj", "src/Worker/"]

# Restore dependencies
RUN dotnet restore "src/Api/EnterpriseTemplate.Api.csproj"

# Copy all source code
COPY ["src/", "src/"]

# Build application
RUN dotnet build "src/Api/EnterpriseTemplate.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "src/Api/EnterpriseTemplate.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EnterpriseTemplate.Api.dll"]
