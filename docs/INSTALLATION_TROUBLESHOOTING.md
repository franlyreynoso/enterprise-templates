# Template Installation Troubleshooting Guide

## Overview

This guide helps resolve common issues when installing, uninstalling, or reinstalling the Enterprise Templates.

## Common Issues

### Issue 1: Duplicate Template Registrations

**Symptoms:**
```
Warning: The following templates use the same identity 'Enterprise.Ui.ConfigurableTemplate':
  • 'Enterprise Blazor UI Template' from 'c:\...\templates\web-template'
  • 'Enterprise Blazor UI Template' from 'C:\...\templates\web-template'
```

**Cause:**
- Template installed multiple times with different path casing (Windows path case sensitivity)
- Installing from repository root instead of individual template directories
- Previous installation not properly cleaned up before reinstalling

**Solution:**
```powershell
# 1. Clean uninstall all templates
.\uninstall-all.ps1

# 2. Reinstall templates
.\install-all.ps1
```

### Issue 2: Path Not Found Errors

**Symptoms:**
```
Warning: Failed to scan C:\...\templates\ui-template.
Details: Template package location C:\...\templates\ui-template is not supported, or doesn't exist.
```

**Cause:**
- Old references to `ui-template` (renamed to `web-template`)
- Template cache contains stale references
- Installing from incorrect directory

**Solution:**
```powershell
# Uninstall all templates and clean cache
.\uninstall-all.ps1

# Verify no enterprise templates remain
dotnet new list | Select-String "enterprise"

# Reinstall
.\install-all.ps1
```

### Issue 3: "Template Package Not Found"

**Symptoms:**
```
The template package 'C:\...\enterprise-templates' is not found.
```

**Cause:**
- Attempting to install from repository root
- .NET template system recursively scans and finds multiple templates

**Solution:**
- ⚠️ **Never run** `dotnet new install .` from repository root
- ✅ **Always use** `.\install-all.ps1` script
- ✅ Or install individual templates from their specific directories:
  ```powershell
  cd templates/web-template
  dotnet new install .
  ```

### Issue 4: "Template Already Installed"

**Symptoms:**
```
WARNING: ⚠️ Enterprise Web Template may already be installed. Use -Force to reinstall.
```

**Cause:**
- Template is already in the dotnet template cache

**Solution:**
```powershell
# Option 1: Force reinstall (recommended for updates)
.\install-all.ps1 -Force

# Option 2: Keep existing installation
# No action needed - templates are already available
```

## Best Practices

### ✅ Do's

1. **Use the Install Script**
   ```powershell
   .\install-all.ps1
   ```

2. **Force Reinstall When Updating**
   ```powershell
   .\install-all.ps1 -Force
   ```

3. **Clean Uninstall Before Major Changes**
   ```powershell
   .\uninstall-all.ps1
   .\install-all.ps1
   ```

4. **Install Individual Templates**
   ```powershell
   cd templates/web-template
   dotnet new install .
   ```

### ❌ Don'ts

1. **Don't Install from Repository Root**
   ```powershell
   # ❌ WRONG - causes duplicate registrations
   dotnet new install .
   ```

2. **Don't Use Relative Paths with Mixed Casing**
   - The install script normalizes paths to prevent case sensitivity issues

3. **Don't Manually Edit Template Cache**
   - Use `dotnet new uninstall` or the provided scripts

## Scripts Reference

### install-all.ps1

Installs all three templates with proper path normalization.

**Basic Usage:**
```powershell
.\install-all.ps1
```

**Options:**
- `-Force`: Uninstall existing templates before reinstalling
- `-Verbose`: Show detailed installation output

**Examples:**
```powershell
# Standard installation
.\install-all.ps1

# Force reinstall (for updates)
.\install-all.ps1 -Force

# Verbose output
.\install-all.ps1 -Verbose

# Force reinstall with verbose output
.\install-all.ps1 -Force -Verbose
```

### uninstall-all.ps1

Removes all enterprise templates from the dotnet template cache.

**Basic Usage:**
```powershell
.\uninstall-all.ps1
```

**Options:**
- `-Verbose`: Show detailed uninstallation output

**Examples:**
```powershell
# Standard uninstall
.\uninstall-all.ps1

# Verbose output
.\uninstall-all.ps1 -Verbose
```

## Advanced Troubleshooting

### Finding Template Installation Locations

```powershell
# List all installed template packages
dotnet new uninstall

# List all templates with their packages
dotnet new list --columns-all
```

### Manually Uninstalling a Specific Template

```powershell
# Get the exact path from 'dotnet new uninstall'
dotnet new uninstall "C:\path\to\template"
```

### Clearing Template Cache (Nuclear Option)

⚠️ **Warning:** This removes ALL installed templates, not just enterprise templates.

**Windows:**
```powershell
Remove-Item -Recurse -Force "$env:USERPROFILE\.templateengine"
```

**macOS/Linux:**
```bash
rm -rf ~/.templateengine
```

## Verification

After installation, verify templates are correctly installed:

```powershell
# Check installed templates
dotnet new list | Select-String "enterprise"

# Expected output:
# Enterprise Blazor UI Template                 blazor-enterprise           [C#]
# Enterprise Clean Architecture Template        enterprise-clean            [C#]
# Enterprise Fullstack Orchestrator             enterprise-fullstack        [C#]
```

## Getting Help

If issues persist:

1. Check the main [README.md](../README.md) for updates
2. Review the [Troubleshooting section](../README.md#-troubleshooting) in README
3. Ensure you have .NET 9.0 SDK or later installed
4. Try the nuclear option (clear template cache) and reinstall
5. Open an issue on GitHub with:
   - Output of `dotnet --version`
   - Output of `dotnet new uninstall`
   - Complete error messages
   - Steps you've already tried

## See Also

- [README.md](../README.md) - Main documentation
- [ARCHITECTURE.md](ARCHITECTURE.md) - System architecture
- [BACKEND-TEMPLATE.md](BACKEND-TEMPLATE.md) - Backend template details
