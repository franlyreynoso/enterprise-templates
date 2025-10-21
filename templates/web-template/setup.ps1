#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Web project post-generation configuration script

.DESCRIPTION
    This script configures your generated Enterprise UI project.
    
    Phase: PROJECT CONFIGURATION (runs AFTER template generation)
    Scope: Single-project setup
    
    This script:
    1. Detects enabled features from your project structure
    2. Generates appsettings.json files based on detected features
    3. Validates environment files (.env.dev, .env.staging, .env.prod)
    4. Provides comprehensive next steps and documentation

.PARAMETER Help
    Show this help message

.NOTES
    This is NOT the same as the fullstack-orchestrator setup.ps1 which runs
    BEFORE generation to coordinate multiple template creation.
#>

param(
    [switch]$Help
)

if ($Help) {
    Get-Help $PSCommandPath -Detailed
    exit 0
}

Write-Host "🚀 Setting up your Enterprise UI project..." -ForegroundColor Green
Write-Host ""

# Check environment files (matches backend template pattern)
$envFiles = @(".env.dev", ".env.staging", ".env.prod")
$missingFiles = @()

foreach ($envFile in $envFiles) {
    if (-not (Test-Path $envFile)) {
        $missingFiles += $envFile
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "📝 Environment files status:" -ForegroundColor Yellow
    foreach ($missing in $missingFiles) {
        Write-Host "   ⚠️  $missing - not found (will be created during template generation)" -ForegroundColor Yellow
    }

    if (Test-Path ".env.template") {
        Write-Host "📄 .env.template available for reference" -ForegroundColor Green
    }
    Write-Host ""
}
else {
    Write-Host "✅ All environment files present (.env.dev, .env.staging, .env.prod)" -ForegroundColor Green
    Write-Host ""
}

# Find the main application project (not the Enterprise.Ui.* libraries)
$appProject = Get-ChildItem -Path "src" -Directory | Where-Object { $_.Name -notlike "Enterprise.Ui.*" } | Select-Object -First 1
if ($appProject) {
    $projectName = $appProject.Name
    $appPath = "src/$projectName"
    $mergeScriptPath = "$appPath/merge-config.ps1"
}
else {
    Write-Host "  ❌ Could not find main application project" -ForegroundColor Red
    return
}

# Detect enabled features from project structure
$EnableAuth = Test-Path "src/Enterprise.Ui.Auth"
$EnableHttpResilience = Test-Path "src/Enterprise.Ui.Http"
$EnableApiClient = Test-Path "src/Enterprise.Ui.ApiClient"
$EnableObservability = Test-Path "src/Enterprise.Ui.Observability"
$EnableFeatureFlags = Test-Path "src/Enterprise.Ui.FeatureFlags"
$EnableSecurity = Test-Path "src/Enterprise.Ui.Security"
$EnableI18n = Test-Path "$appPath/Resources"
$EnableTesting = Test-Path "tests"
$EnableCICD = Test-Path ".github/workflows"
$EnableDocker = Test-Path "Dockerfile"

# Detect backend integration features
$EnableBackendIntegration = Test-Path "$appPath/Config/backend*.json"
$EnableDevEnvironment = Test-Path "docker-compose.integrated.yml"

Write-Host "📋 Detected Features:" -ForegroundColor Yellow
Write-Host "  Authentication: $(if($EnableAuth){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  HTTP Resilience: $(if($EnableHttpResilience){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  API Client: $(if($EnableApiClient){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Observability: $(if($EnableObservability){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Feature Flags: $(if($EnableFeatureFlags){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Security Headers: $(if($EnableSecurity){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Internationalization: $(if($EnableI18n){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Testing: $(if($EnableTesting){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  CI/CD: $(if($EnableCICD){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Docker: $(if($EnableDocker){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Backend Integration: $(if($EnableBackendIntegration){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host "  Dev Environment: $(if($EnableDevEnvironment){'✅ Enabled'}else{'❌ Disabled'})"
Write-Host ""

# Generate configuration files if merge script exists
if (Test-Path $mergeScriptPath) {
    Write-Host "⚙️ Generating configuration files..." -ForegroundColor Yellow

    # Generate Production configuration
    $prodParams = @{
        Environment              = "Production"
        OutputPath               = "appsettings.json"
        EnableAuth               = $EnableAuth
        EnableHttpResilience     = $EnableHttpResilience
        EnableFeatureFlags       = $EnableFeatureFlags
        EnableObservability      = $EnableObservability
        EnableI18n               = $EnableI18n
        EnableSecurity           = $EnableSecurity
        EnableBackendIntegration = $EnableBackendIntegration
    }

    try {
        Push-Location $appPath

        & "./merge-config.ps1" @prodParams
        Pop-Location
        Write-Host "  ✅ Generated appsettings.json" -ForegroundColor Green
    }
    catch {
        Pop-Location
        Write-Host "  ❌ Failed to generate appsettings.json: $_" -ForegroundColor Red
    }

    # Generate Development configuration
    $devParams = @{
        Environment              = "Development"
        OutputPath               = "appsettings.Development.json"
        EnableAuth               = $EnableAuth
        EnableHttpResilience     = $EnableHttpResilience
        EnableFeatureFlags       = $EnableFeatureFlags
        EnableObservability      = $EnableObservability
        EnableI18n               = $EnableI18n
        EnableSecurity           = $EnableSecurity
        EnableBackendIntegration = $EnableBackendIntegration
    }

    try {
        Push-Location $appPath

        & "./merge-config.ps1" @devParams
        Pop-Location
        Write-Host "  ✅ Generated appsettings.Development.json" -ForegroundColor Green
    }
    catch {
        Pop-Location
        Write-Host "  ❌ Failed to generate appsettings.Development.json: $_" -ForegroundColor Red
    }
}
else {
    Write-Host "  ⚠️ Configuration merge script not found at $mergeScriptPath, using default settings" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🎉 Setup complete!" -ForegroundColor Green
Write-Host ""

# Next steps
Write-Host "📝 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Review and customize configuration files:"
Write-Host "   - $appPath/appsettings.json"
Write-Host "   - $appPath/appsettings.Development.json"
Write-Host ""

if ($EnableAuth) {
    Write-Host "2. Configure authentication:"
    Write-Host "   - Update Auth.Authority with your identity provider"
    Write-Host "   - Set Auth.ClientId for your application"
    Write-Host ""
}

if ($EnableObservability) {
    Write-Host "3. Configure observability:"
    Write-Host "   - Set Observability.OtlpEndpoint for your telemetry collector"
    Write-Host "   - Update Observability.ServiceName if needed"
    Write-Host ""
}

if ($EnableBackendIntegration) {
    Write-Host "4. Configure backend integration:"
    Write-Host "   - Update Backend.ApiUrl with your API base URL"
    Write-Host "   - Configure authentication provider settings"
    Write-Host "   - Generate API client using: cd $appPath && dotnet build"
    Write-Host ""
}

Write-Host "5. Choose your development environment (matches backend pattern):"
Write-Host ""
Write-Host "   🐳 Environment-Specific Development:"
Write-Host "   make up ENV=dev         # Start development environment"
Write-Host "   make up ENV=staging     # Start staging environment"
Write-Host "   make up ENV=prod        # Start production environment"
Write-Host "   make down ENV=dev       # Stop environment"
Write-Host ""
Write-Host "   🎯 Quick Commands (shortcuts):"
Write-Host "   make up-dev             # Start development"
Write-Host "   make up-staging         # Start staging"
Write-Host "   make up-prod            # Start production"
Write-Host ""
Write-Host "   🌟 Full-Stack Development (UI + Backend):"
Write-Host "   make up-integrated      # Start UI + Backend + Database + Observability"
Write-Host "   make logs-integrated    # View all integrated logs"
Write-Host "   make down-integrated    # Stop full stack"
Write-Host ""
Write-Host "   🏃 Local Development (No Docker):"
Write-Host "   make restore            # Restore packages"
Write-Host "   make run               # Run locally"
Write-Host ""
Write-Host "   🧪 Testing:"
Write-Host "   make test              # Run all tests"
Write-Host "   make test-unit         # Unit tests only"
Write-Host "   make test-e2e          # End-to-end tests"
Write-Host ""

Write-Host "6. View your application:"
Write-Host "   🌐 https://localhost:7000 (UI)"
Write-Host "   💚 https://localhost:7000/health (Health checks)"
Write-Host "   🗄️  localhost:5432 (PostgreSQL database)"
if ($EnableBackendIntegration) {
    Write-Host "   🔧 https://localhost:8001 (API - integrated mode)"
}
if ($EnableObservability) {
    Write-Host "   📊 http://localhost:16686 (Jaeger tracing)"
    Write-Host "   📝 http://localhost:5341 (Seq centralized logging)"
    Write-Host "   📈 http://localhost:9090 (Prometheus metrics)"
    Write-Host "   📋 http://localhost:3000 (Grafana dashboards)"
    Write-Host "   🐘 http://localhost:5050 (pgAdmin - dev only)"
}
Write-Host ""

Write-Host "8. VS Code Integration:"
Write-Host "   - Command Palette (Ctrl+Shift+P) → 'Tasks: Run Task'"
Write-Host "   - Select from available tasks like '🚀 Start Development Environment'"
Write-Host "   - Or use Dev Container: 'Dev Containers: Reopen in Container'"
Write-Host ""

Write-Host "8. Available Commands (matches backend template):"
Write-Host "   make help              # Show all available commands"
Write-Host "   make setup             # Re-run this setup"
Write-Host "   make config ENV=dev    # Regenerate configuration for environment"
Write-Host "   make logs ENV=dev      # View environment logs"
Write-Host "   make logs-seq          # View centralized logs (Seq)"
Write-Host "   make clean             # Clean Docker environment"
Write-Host ""

Write-Host "📚 Documentation:" -ForegroundColor Cyan
Write-Host "   - Configuration: $appPath/CONFIG.md"
Write-Host "   - Architecture: ARCHITECTURE.md"
Write-Host "   - Security: SECURITY.md"
Write-Host ""

Write-Host "Happy coding! 🚀" -ForegroundColor Green