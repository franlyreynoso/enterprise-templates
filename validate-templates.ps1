#!/usr/bin/env pwsh
# Validation Script for Enterprise Templates Repository
# Checks for common issues and inconsistencies

Write-Host "🔍 Enterprise Templates Repository Validation" -ForegroundColor Cyan
Write-Host "=============================================`n" -ForegroundColor Cyan

$issues = @()
$warnings = @()
$passed = @()

# Check 1: No backup files
Write-Host "📁 Checking for backup files..." -ForegroundColor Yellow
$backupFiles = Get-ChildItem -Path . -Recurse -Include *.backup,*.bak,*~ -ErrorAction SilentlyContinue
if ($backupFiles) {
    $issues += "Found backup files: $($backupFiles.Count) files"
    $backupFiles | ForEach-Object { Write-Host "  ❌ $_" -ForegroundColor Red }
} else {
    $passed += "No backup files found"
    Write-Host "  ✅ No backup files found" -ForegroundColor Green
}

# Check 2: Environment variable consistency
Write-Host "`n🔧 Checking environment variable naming..." -ForegroundColor Yellow
$envFiles = Get-ChildItem -Path templates -Recurse -Filter ".env.*" | Where-Object { $_.Name -notmatch "template|example" }
$portVars = @{}

foreach ($file in $envFiles) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -match "JAEGER_WEB_PORT") {
        $issues += "File $($file.Name) uses JAEGER_WEB_PORT instead of JAEGER_UI_PORT"
    }
    if ($content -match "MAILHOG_WEB_PORT") {
        $issues += "File $($file.Name) uses MAILHOG_WEB_PORT instead of MAILHOG_UI_PORT"
    }
}

if ($issues.Count -eq 0 -or -not ($issues -match "JAEGER|MAILHOG")) {
    $passed += "Environment variable naming is consistent"
    Write-Host "  ✅ Port variable naming is consistent" -ForegroundColor Green
}

# Check 3: Makefile corruption
Write-Host "`n📝 Checking Makefiles..." -ForegroundColor Yellow
$makefiles = Get-ChildItem -Path templates -Recurse -Filter "Makefile"
foreach ($makefile in $makefiles) {
    $lines = Get-Content $makefile.FullName
    $duplicateLines = $lines | Group-Object | Where-Object { $_.Count -gt 1 -and $_.Name -match "ifeq|SHELL" }
    
    if ($duplicateLines) {
        $issues += "Makefile $($makefile.FullName) has duplicate lines (possible merge conflict)"
    }
    
    # Check for incomplete sections
    if ((Get-Content $makefile.FullName -Raw) -match "unchanged\)") {
        $issues += "Makefile $($makefile.FullName) has incomplete sections"
    }
}

if ($issues.Count -eq 0 -or -not ($issues -match "Makefile")) {
    $passed += "Makefiles are clean and complete"
    Write-Host "  ✅ Makefiles are clean" -ForegroundColor Green
}

# Check 4: Documentation references
Write-Host "`n📚 Checking documentation references..." -ForegroundColor Yellow
$readmeContent = Get-Content README.md -Raw
$archContent = Get-Content docs/ARCHITECTURE.md -Raw

if ($readmeContent -match "ui-template" -or $archContent -match "ui-template") {
    $warnings += "Documentation still references 'ui-template' instead of 'web-template'"
}

$docsToCheck = @(
    @{Path = "docs/UI-TEMPLATE.md"; Referenced = $readmeContent -match "UI-TEMPLATE\.md"},
    @{Path = "docs/ORCHESTRATOR.md"; Referenced = $readmeContent -match "ORCHESTRATOR\.md"}
)

foreach ($doc in $docsToCheck) {
    if ($doc.Referenced -and -not (Test-Path $doc.Path)) {
        $warnings += "Documentation references $($doc.Path) but file doesn't exist"
    }
}

if ($warnings.Count -eq 0) {
    $passed += "All documentation references are valid"
    Write-Host "  ✅ Documentation references are valid" -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Some documentation references need attention" -ForegroundColor Yellow
}

# Check 5: Template.json consistency
Write-Host "`n🎯 Checking template.json files..." -ForegroundColor Yellow
$templateJsons = Get-ChildItem -Path templates -Recurse -Filter "template.json"
foreach ($json in $templateJsons) {
    try {
        $templateConfig = Get-Content $json.FullName -Raw | ConvertFrom-Json
        if ($templateConfig.shortName) {
            Write-Host "  📦 $($json.Directory.Parent.Name): $($templateConfig.shortName)" -ForegroundColor Gray
        }
    } catch {
        $issues += "Failed to parse template.json: $($json.FullName)"
    }
}
$passed += "Template.json files are valid JSON"

# Check 6: Git ignore present
Write-Host "`n🚫 Checking .gitignore..." -ForegroundColor Yellow
if (Test-Path ".gitignore") {
    $gitignoreContent = Get-Content .gitignore -Raw
    if ($gitignoreContent -match "\.backup") {
        $passed += "Root .gitignore excludes backup files"
        Write-Host "  ✅ Root .gitignore present and configured" -ForegroundColor Green
    } else {
        $warnings += "Root .gitignore doesn't exclude .backup files"
    }
} else {
    $issues += "Root .gitignore is missing"
}

# Summary
Write-Host "`n" + "="*50 -ForegroundColor Cyan
Write-Host "📊 Validation Summary" -ForegroundColor Cyan
Write-Host "="*50 -ForegroundColor Cyan

Write-Host "`n✅ Passed: $($passed.Count)" -ForegroundColor Green
foreach ($p in $passed) {
    Write-Host "  • $p" -ForegroundColor Green
}

if ($warnings.Count -gt 0) {
    Write-Host "`n⚠️  Warnings: $($warnings.Count)" -ForegroundColor Yellow
    foreach ($w in $warnings) {
        Write-Host "  • $w" -ForegroundColor Yellow
    }
}

if ($issues.Count -gt 0) {
    Write-Host "`n❌ Issues: $($issues.Count)" -ForegroundColor Red
    foreach ($i in $issues) {
        Write-Host "  • $i" -ForegroundColor Red
    }
    Write-Host "`n🔴 Validation FAILED - Please fix the issues above" -ForegroundColor Red
    exit 1
} else {
    Write-Host "`n🎉 All critical checks passed!" -ForegroundColor Green
    if ($warnings.Count -gt 0) {
        Write-Host "⚠️  Please review warnings above" -ForegroundColor Yellow
        exit 0
    }
    exit 0
}
