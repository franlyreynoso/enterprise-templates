#!/usr/bin/env pwsh
# Enterprise Templates - Uninstall All Templates
# Removes all enterprise templates from the dotnet template cache

param(
    [switch]$Verbose = $false
)

Write-Host "🗑️  Enterprise Templates Uninstallation" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

# Check prerequisites
Write-Host "🔍 Checking prerequisites..." -ForegroundColor Yellow

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "❌ .NET SDK not found. Please install .NET SDK."
    exit 1
}

$dotnetVersion = & dotnet --version
Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Function to uninstall template
function Uninstall-Template {
    param(
        [string]$Name,
        [string]$Path,
        [string]$Description
    )

    Write-Host "🗑️  Uninstalling $Name..." -ForegroundColor Blue

    try {
        # Normalize path to avoid case sensitivity issues
        $normalizedPath = (Resolve-Path $Path -ErrorAction SilentlyContinue).Path
        if (-not $normalizedPath) {
            $normalizedPath = (Get-Item $Path -ErrorAction SilentlyContinue).FullName
        }
        
        if (-not $normalizedPath) {
            Write-Host "⚠️  Path not found: $Path" -ForegroundColor Yellow
            return
        }

        $output = & dotnet new uninstall $normalizedPath 2>&1
        $exitCode = $LASTEXITCODE

        if ($exitCode -eq 0) {
            Write-Host "✅ $Name uninstalled successfully!" -ForegroundColor Green
            if ($Verbose) {
                Write-Host "   $Description" -ForegroundColor Gray
            }
        } else {
            # Check if template wasn't installed
            if ($output -match "is not found" -or $output -match "not installed") {
                Write-Host "ℹ️  $Name was not installed" -ForegroundColor Gray
            } else {
                Write-Warning "⚠️  Failed to uninstall $Name"
                if ($Verbose) {
                    Write-Host $output -ForegroundColor Gray
                }
            }
        }
    }
    catch {
        Write-Warning "⚠️  Error uninstalling ${Name}: ${_}"
    }
}

Write-Host "📋 Finding installed enterprise templates..." -ForegroundColor Yellow
Write-Host ""

# List all installed templates to identify enterprise ones
$installedTemplates = & dotnet new uninstall 2>&1
Write-Host "Currently installed template packages:" -ForegroundColor Gray
Write-Host $installedTemplates -ForegroundColor Gray
Write-Host ""

# Uninstall templates in reverse dependency order
try {
    # 1. Fullstack Orchestrator (depends on others)
    if (Test-Path ".\templates\fullstack-orchestrator") {
        Uninstall-Template -Name "Enterprise Fullstack Orchestrator" -Path ".\templates\fullstack-orchestrator" -Description "Meta-template that coordinates Web and backend templates"
    }

    # 2. Backend Template
    if (Test-Path ".\templates\backend-template") {
        Uninstall-Template -Name "Enterprise Backend Template" -Path ".\templates\backend-template" -Description "Clean Architecture API template with CQRS"
    }

    # 3. Web Template
    if (Test-Path ".\templates\web-template") {
        Uninstall-Template -Name "Enterprise Web Template" -Path ".\templates\web-template" -Description "Blazor Server enterprise template with backend integration"
    }

    # Try to uninstall old naming variations if they exist
    Write-Host ""
    Write-Host "🔍 Checking for old template installations..." -ForegroundColor Yellow
    
    # Try old ui-template path (in case it was installed before)
    if (Test-Path ".\templates\ui-template") {
        Uninstall-Template -Name "Old UI Template (ui-template)" -Path ".\templates\ui-template" -Description "Legacy path reference"
    }

    Write-Host ""
    Write-Host "🎉 Uninstallation completed!" -ForegroundColor Green
    Write-Host ""
    
    # Show remaining templates
    Write-Host "📋 Remaining Template Packages:" -ForegroundColor Cyan
    Write-Host ""
    & dotnet new uninstall
    
    Write-Host ""
    Write-Host "💡 Tips:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  • To reinstall templates, run: .\install-all.ps1" -ForegroundColor Gray
    Write-Host "  • To install with force reinstall: .\install-all.ps1 -Force" -ForegroundColor Gray
    Write-Host ""
}
catch {
    Write-Error "❌ Uninstallation failed: ${_}"
    Write-Host ""
    Write-Host "🛠️  Troubleshooting:" -ForegroundColor Yellow
    Write-Host "  - Try running: dotnet new uninstall" -ForegroundColor Gray
    Write-Host "  - Manually uninstall using the paths shown above" -ForegroundColor Gray
    Write-Host "  - Check for permission issues" -ForegroundColor Gray
    exit 1
}

Write-Host "✨ Enterprise Templates uninstallation completed!" -ForegroundColor Green
