#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Enterprise Fullstack project generation coordinator

.DESCRIPTION
    This script is part of the enterprise-fullstack meta-template.
    It coordinates the generation of both Web (Blazor) and API (Clean Architecture) projects.
    
    Phase: PROJECT GENERATION (runs BEFORE projects are created)
    Scope: Multi-project coordination
    
    This script:
    1. Checks prerequisites (.NET SDK, Docker)
    2. Generates Web project using blazor-enterprise template
    3. Generates API project using enterprise-clean template
    4. Optionally starts the full development environment

.NOTES
    This is NOT the same as the web-template setup.ps1 which runs AFTER generation
    to configure individual projects.
#>

param(
    [string]$WebProjectName = "WEB_PROJECT_NAME",
    [string]$APIProjectName = "API_PROJECT_NAME",
    [string]$Cloud = "CLOUD_PROVIDER",
    [string]$Database = "DATABASE_PROVIDER",
    [string]$MessageBus = "MESSAGE_BUS",
    [string]$LoggingProvider = "LOGGING_PROVIDER",
    [switch]$EnableCaching = $true,
    [switch]$EnableObservability = $true,
    [switch]$EnableAuth = $true,
    [switch]$SkipGeneration = $false,
    [switch]$StartEnvironment = $false
)

Write-Host "🚀 Enterprise Fullstack Setup" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Check prerequisites
Write-Host "🔍 Checking prerequisites..." -ForegroundColor Yellow

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "❌ .NET SDK not found. Please install .NET 9.0 SDK."
    exit 1
}

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "❌ Docker not found. Please install Docker Desktop."
    exit 1
}

Write-Host "✅ Prerequisites satisfied!" -ForegroundColor Green
Write-Host ""

# Display configuration
Write-Host "📋 Configuration:" -ForegroundColor Cyan
Write-Host "  Web Project: $WebProjectName" -ForegroundColor White
Write-Host "  API Project: $APIProjectName" -ForegroundColor White
Write-Host "  Cloud: $Cloud" -ForegroundColor White
Write-Host "  Database: $Database" -ForegroundColor White
Write-Host "  Message Bus: $MessageBus" -ForegroundColor White
Write-Host "  Logging: $LoggingProvider" -ForegroundColor White
Write-Host "  Caching: $(if ($EnableCaching) { "✅ Enabled" } else { "❌ Disabled" })" -ForegroundColor $(if ($EnableCaching) { "Green" } else { "Red" })
Write-Host "  Observability: $(if ($EnableObservability) { "✅ Enabled" } else { "❌ Disabled" })" -ForegroundColor $(if ($EnableObservability) { "Green" } else { "Red" })
Write-Host "  Authentication: $(if ($EnableAuth) { "✅ Enabled" } else { "❌ Disabled" })" -ForegroundColor $(if ($EnableAuth) { "Green" } else { "Red" })
Write-Host ""

if (-not $SkipGeneration) {
    # Generate Web project
    if (Test-Path $WebProjectName) {
        Write-Host "⚠️ Web project '$WebProjectName' already exists, skipping generation..." -ForegroundColor Yellow
    }
    else {
        Write-Host "� Generating Web project..." -ForegroundColor Blue
        $webArgs = @(
            "new", "blazor-enterprise", "-n", $WebProjectName,
            "--IncludeAuth", $EnableAuth.ToString().ToLower(),
            "--IncludeObservability", $EnableObservability.ToString().ToLower(),
            "--BackendIntegration", "true"
        )

        & dotnet @webArgs

        if ($LASTEXITCODE -ne 0) {
            Write-Error "❌ Failed to generate Web project. Make sure 'blazor-enterprise' template is installed."
            exit 1
        }

        Write-Host "✅ Web project generated!" -ForegroundColor Green
    }

    # Generate API project
    if (Test-Path $APIProjectName) {
        Write-Host "⚠️ API project '$APIProjectName' already exists, skipping generation..." -ForegroundColor Yellow
    }
    else {
        Write-Host "⚙️ Generating API project..." -ForegroundColor Magenta
        $apiArgs = @(
            "new", "enterprise-clean", "-n", $APIProjectName,
            "--cloud", $Cloud,
            "--db", $Database,
            "--bus", $MessageBus,
            "--caching", $EnableCaching.ToString().ToLower(),
            "--logging", $LoggingProvider
        )

        & dotnet @apiArgs

        if ($LASTEXITCODE -ne 0) {
            Write-Error "❌ Failed to generate API project. Make sure 'enterprise-clean' template is installed."
            exit 1
        }

        Write-Host "✅ API project generated!" -ForegroundColor Green
    }

    Write-Host ""
    Write-Host "🎉 Project generation completed!" -ForegroundColor Green
}

# Start environment if requested
if ($StartEnvironment) {
    Write-Host "🚀 Starting development environment..." -ForegroundColor Green
    & make up-fullstack

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Development environment started successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "🌐 Access your applications:" -ForegroundColor Cyan
        Write-Host "  Web Application: http://localhost:3000" -ForegroundColor White
        Write-Host "  API: http://localhost:5000" -ForegroundColor White
        Write-Host "  API Documentation: http://localhost:5000/swagger" -ForegroundColor White
        Write-Host "  Database Admin: http://localhost:5050 (admin@example.com / admin)" -ForegroundColor White
        Write-Host "  Message Queue: http://localhost:15672 (app / app)" -ForegroundColor White
        Write-Host "  Distributed Tracing: http://localhost:16686" -ForegroundColor White
        Write-Host "  Centralized Logging: http://localhost:5341" -ForegroundColor White
    }
    else {
        Write-Error "❌ Failed to start development environment."
        exit 1
    }
}
else {
    Write-Host "📚 Next Steps:" -ForegroundColor Cyan
    Write-Host "  1. Start development environment: make up-fullstack" -ForegroundColor White
    Write-Host "  2. View logs: make logs" -ForegroundColor White
    Write-Host "  3. Check health: make health" -ForegroundColor White
    Write-Host "  4. Stop environment: make down" -ForegroundColor White
}

Write-Host ""
Write-Host "📖 For more information:" -ForegroundColor Yellow
Write-Host "  - Run 'make help' to see all available commands" -ForegroundColor White
Write-Host "  - Check README.md for detailed documentation" -ForegroundColor White
Write-Host "  - Visit Web project: cd $WebProjectName" -ForegroundColor White
Write-Host "  - Visit API project: cd $APIProjectName" -ForegroundColor White

Write-Host ""
Write-Host "🎊 Enterprise Fullstack setup complete!" -ForegroundColor Green