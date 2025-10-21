# Enterprise Blazor Template - Installation and Usage Scripts

# PowerShell script for Windows
Write-Host "Enterprise Blazor UI Template - Installation Script" -ForegroundColor Green

# Function to install template
function Install-Template {
    param(
        [string]$TemplatePath = ".",
        [switch]$Force
    )

    Write-Host "Installing Enterprise Blazor UI Template..." -ForegroundColor Yellow

    if ($Force) {
        Write-Host "Uninstalling existing template..." -ForegroundColor Yellow
        dotnet new uninstall $TemplatePath
    }

    dotnet new install $TemplatePath

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Template installed successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Available presets:" -ForegroundColor Cyan
        Write-Host "  - Full        : Complete enterprise application" -ForegroundColor White
        Write-Host "  - Standard    : Common enterprise features" -ForegroundColor White
        Write-Host "  - Microservice: Microservice-optimized" -ForegroundColor White
        Write-Host "  - Minimal     : Basic Blazor with MudBlazor" -ForegroundColor White
        Write-Host ""
        Write-Host "Example usage:" -ForegroundColor Cyan
        Write-Host "  dotnet new blazor-enterprise -n MyApp --TemplatePreset Full" -ForegroundColor White
        Write-Host ""
        Write-Host "For detailed usage instructions, see TEMPLATE-USAGE.md" -ForegroundColor Yellow
    }
    else {
        Write-Host "❌ Template installation failed!" -ForegroundColor Red
    }
}

# Function to create sample projects
function New-SampleProjects {
    Write-Host "Creating sample projects..." -ForegroundColor Yellow

    $samples = @(
        @{ Name = "FullEnterpriseApp"; Preset = "Full"; Description = "Complete enterprise application" },
        @{ Name = "StandardBusinessApp"; Preset = "Standard"; Description = "Standard business application" },
        @{ Name = "MicroserviceUI"; Preset = "Microservice"; Description = "Microservice UI component" },
        @{ Name = "MinimalDemo"; Preset = "Minimal"; Description = "Simple demo application" }
    )

    foreach ($sample in $samples) {
        Write-Host "Creating $($sample.Name) ($($sample.Description))..." -ForegroundColor Cyan
        dotnet new blazor-enterprise -n $sample.Name --TemplatePreset $sample.Preset --force
    }

    Write-Host "✅ Sample projects created successfully!" -ForegroundColor Green
}

# Function to test template variations
function Test-TemplateVariations {
    Write-Host "Testing template variations..." -ForegroundColor Yellow

    $testCases = @(
        @{ Name = "AuthOnly"; Preset = "Custom"; Features = "--IncludeAuth true --IncludeHttpResilience false --IncludeObservability false" },
        @{ Name = "NoTesting"; Preset = "Custom"; Features = "--IncludeAuth true --IncludeHttpResilience true --IncludeTesting false" },
        @{ Name = "ObservabilityFocus"; Preset = "Custom"; Features = "--IncludeObservability true --IncludeAuth false --IncludeHttpResilience true" }
    )

    foreach ($test in $testCases) {
        Write-Host "Testing: $($test.Name)" -ForegroundColor Cyan
        $command = "dotnet new blazor-enterprise -n Test$($test.Name) --TemplatePreset $($test.Preset) $($test.Features) --dry-run"
        Invoke-Expression $command

        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ $($test.Name) configuration valid" -ForegroundColor Green
        }
        else {
            Write-Host "❌ $($test.Name) configuration failed" -ForegroundColor Red
        }
    }
}

# Main menu
function Show-Menu {
    Clear-Host
    Write-Host "============================================" -ForegroundColor Green
    Write-Host "Enterprise Blazor UI Template Manager" -ForegroundColor Green
    Write-Host "============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "1. Install Template" -ForegroundColor Cyan
    Write-Host "2. Reinstall Template (Force)" -ForegroundColor Cyan
    Write-Host "3. Create Sample Projects" -ForegroundColor Cyan
    Write-Host "4. Test Template Variations" -ForegroundColor Cyan
    Write-Host "5. Show Template Help" -ForegroundColor Cyan
    Write-Host "6. List Installed Templates" -ForegroundColor Cyan
    Write-Host "7. Uninstall Template" -ForegroundColor Cyan
    Write-Host "0. Exit" -ForegroundColor Yellow
    Write-Host ""
}

# Main script loop
do {
    Show-Menu
    $choice = Read-Host "Select an option"

    switch ($choice) {
        '1' {
            Install-Template
            Read-Host "Press Enter to continue"
        }
        '2' {
            Install-Template -Force
            Read-Host "Press Enter to continue"
        }
        '3' {
            New-SampleProjects
            Read-Host "Press Enter to continue"
        }
        '4' {
            Test-TemplateVariations
            Read-Host "Press Enter to continue"
        }
        '5' {
            dotnet new blazor-enterprise --help
            Read-Host "Press Enter to continue"
        }
        '6' {
            dotnet new list
            Read-Host "Press Enter to continue"
        }
        '7' {
            Write-Host "Uninstalling template..." -ForegroundColor Yellow
            dotnet new uninstall "."
            Read-Host "Press Enter to continue"
        }
        '0' {
            Write-Host "Goodbye!" -ForegroundColor Green
        }
        default {
            Write-Host "Invalid option. Please try again." -ForegroundColor Red
            Start-Sleep -Seconds 2
        }
    }
} while ($choice -ne '0')