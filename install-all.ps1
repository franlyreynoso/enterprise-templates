#!/usr/bin/env pwsh
# Enterprise Templates - Install All Templates
# Installs UI template, backend template, and fullstack orchestrator

param(
    [switch]$Force = $false,
    [switch]$Verbose = $false
)

Write-Host "🚀 Enterprise Templates Installation" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# Check prerequisites
Write-Host "🔍 Checking prerequisites..." -ForegroundColor Yellow

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "❌ .NET SDK not found. Please install .NET 9.0 SDK."
    exit 1
}

$dotnetVersion = & dotnet --version
Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green

# Template installation function
function Install-Template {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Description
    )

    Write-Host "📦 Installing $Name..." -ForegroundColor Blue

    try {
        if ($Force) {
            & dotnet new uninstall $Path 2>$null
        }

        $output = & dotnet new install $Path 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ $Name installed successfully!" -ForegroundColor Green
            if ($Verbose) {
                Write-Host "   $Description" -ForegroundColor Gray
            }
        } else {
            Write-Warning "⚠️ $Name may already be installed. Use -Force to reinstall."
            if ($Verbose) {
                Write-Host $output -ForegroundColor Gray
            }
        }
    }
    catch {
        Write-Error "❌ Failed to install ${Name}: ${_}"
        throw
    }
}

Write-Host ""

# Install templates in dependency order
try {
    # 1. UI Template
    Install-Template -Name "Enterprise UI Template" -Path ".\templates\ui-template" -Description "Blazor Server enterprise template with backend integration"

    # 2. Backend Template (if available - will use external)
    if (Test-Path ".\templates\backend-template\src") {
        Install-Template -Name "Enterprise Backend Template" -Path ".\templates\backend-template" -Description "Clean Architecture API template with CQRS"
    } else {
        Write-Host "📦 Installing Enterprise Backend Template (external)..." -ForegroundColor Blue
        if (Test-Path ".\backend-template-repo") {
            Remove-Item ".\backend-template-repo" -Recurse -Force
        }

        # Clone or copy the backend template (temporary solution)
        Write-Host "ℹ️ Backend template should be installed separately from franlyreynoso/EnterpriseTemplate" -ForegroundColor Yellow
        Write-Host "   Run: git clone https://github.com/franlyreynoso/EnterpriseTemplate backend-template-repo" -ForegroundColor Gray
        Write-Host "   Then: dotnet new install ./backend-template-repo" -ForegroundColor Gray
    }

    # 3. Fullstack Orchestrator
    Install-Template -Name "Enterprise Fullstack Orchestrator" -Path ".\templates\fullstack-orchestrator" -Description "Meta-template that coordinates UI and backend templates"

    Write-Host ""
    Write-Host "🎉 All templates installed successfully!" -ForegroundColor Green

    # Show installed templates
    Write-Host ""
    Write-Host "📋 Installed Templates:" -ForegroundColor Cyan
    Write-Host ""

    $templates = & dotnet new list | Select-String -Pattern "enterprise-"
    foreach ($template in $templates) {
        Write-Host "  $template" -ForegroundColor White
    }

    Write-Host ""
    Write-Host "🚀 Quick Start:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  # Generate individual templates:" -ForegroundColor Gray
    Write-Host "  dotnet new enterprise-ui -n MyCompany.UI" -ForegroundColor White
    Write-Host "  dotnet new enterprise-api -n MyCompany.API" -ForegroundColor White
    Write-Host ""
    Write-Host "  # Generate fullstack solution:" -ForegroundColor Gray
    Write-Host "  dotnet new enterprise-fullstack -n MyCompany.Solution" -ForegroundColor White
    Write-Host ""
    Write-Host "📚 Documentation:" -ForegroundColor Yellow
    Write-Host "  - UI Template: docs/UI-TEMPLATE.md" -ForegroundColor Gray
    Write-Host "  - Backend Template: docs/BACKEND-TEMPLATE.md" -ForegroundColor Gray
    Write-Host "  - Orchestrator: docs/ORCHESTRATOR.md" -ForegroundColor Gray

}
catch {
    Write-Error "❌ Installation failed: ${_}"
    Write-Host ""
    Write-Host "🛠️ Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Ensure you have .NET 9.0 SDK installed" -ForegroundColor Gray
    Write-Host "  - Try running with -Force to reinstall existing templates" -ForegroundColor Gray
    Write-Host "  - Check that all template directories contain valid .template.config/template.json files" -ForegroundColor Gray
    exit 1
}

Write-Host ""
Write-Host "✨ Enterprise Templates installation completed!" -ForegroundColor Green