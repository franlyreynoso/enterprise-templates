# Configuration Management Solution

## Problem Solved

The original enterprise template required users to manually configure `appsettings.json` files after template generation, leading to:

- **Manual setup errors** - Users had to manually add/remove configuration sections
- **Configuration bloat** - Files contained unused settings for disabled features
- **Inconsistent environments** - Development vs Production settings were error-prone
- **Maintenance overhead** - Changes required updating multiple files manually

## Solution Overview

We implemented an **intelligent configuration management system** that automatically generates clean, feature-specific configuration files during template instantiation.

### Key Components

1. **Modular Configuration Fragments** (`src/Enterprise.App/Config/`)

   - Individual JSON fragments for each enterprise feature
   - Environment-specific overrides (`.Development.json` variants)
   - Clean separation of concerns

2. **PowerShell Merge Script** (`merge-config.ps1`)

   - Combines base configuration with feature-specific fragments
   - Supports both Production and Development environments
   - Handles nested configuration merging intelligently

3. **Template Post-Actions** (`.template.config/template.json`)

   - Automatically runs configuration merger after template generation
   - Passes feature flags as parameters to merge script
   - Generates both `appsettings.json` and `appsettings.Development.json`

4. **Conditional File Inclusion**
   - Only includes configuration fragments for enabled features
   - Excludes unused fragments to keep generated projects clean
   - Maintains clean project structure

### Configuration Fragments

| Fragment             | Purpose                 | Production Example                      | Development Override            |
| -------------------- | ----------------------- | --------------------------------------- | ------------------------------- |
| `auth.json`          | Authentication settings | HTTPS required, production authority    | HTTP allowed, demo authority    |
| `http.json`          | HTTP resilience         | Conservative timeouts, circuit breakers | Longer timeouts, fewer retries  |
| `features.json`      | Feature flags           | Conservative enablement                 | All features enabled            |
| `observability.json` | OpenTelemetry           | Production endpoint, no console         | Local endpoint, console enabled |
| `security.json`      | Security headers        | Strict CSP, HSTS enabled                | Relaxed for development         |
| `i18n.json`          | Localization            | Multiple cultures                       | Single culture                  |

### Benefits Achieved

✅ **Zero Manual Configuration** - Files generated automatically based on selections
✅ **Clean Configuration** - Only includes settings for enabled features
✅ **Environment-Aware** - Appropriate defaults for Development vs Production
✅ **Maintainable** - Changes to fragments automatically propagate
✅ **Extensible** - Easy to add new configuration fragments
✅ **Consistent** - Eliminates configuration drift between environments

### Usage Examples

#### During Template Creation

```bash
# Full enterprise template with automatic configuration
dotnet new enterprise-ui -n "MyApp" --preset Full
# Result: Generates appsettings.json with all enterprise features configured

# Minimal template with basic configuration
dotnet new enterprise-ui -n "MyApp" --preset Minimal
# Result: Generates appsettings.json with only core settings

# Custom selection with specific features
dotnet new enterprise-ui -n "MyApp" --preset Custom --IncludeAuth --IncludeObservability
# Result: Generates appsettings.json with only Auth and Observability sections
```

#### Manual Regeneration

```powershell
# Regenerate with different feature set
.\merge-config.ps1 -Environment Production -EnableAuth -EnableSecurity

# Development environment with debugging enabled
.\merge-config.ps1 -Environment Development -EnableAuth -EnableObservability -EnableFeatureFlags
```

### Technical Implementation

#### PowerShell Merge Logic

```powershell
# Load base configuration
$baseConfig = @{
    "Logging" = @{ "LogLevel" = @{ "Default" = "Information" } }
    "AllowedHosts" = "*"
}

# Merge feature-specific fragments
if ($EnableAuth) {
    $authConfig = Load-ConfigFragment -FragmentName "auth" -Environment $Environment
    Merge-Configuration -Target $baseConfig -Source $authConfig
}

# Deep merge function handles nested objects recursively
function Merge-Configuration {
    param($Target, $Source)
    foreach ($key in $Source.Keys) {
        if ($Target.ContainsKey($key) -and $Target[$key] -is [hashtable]) {
            Merge-Configuration -Target $Target[$key] -Source $Source[$key]
        } else {
            $Target[$key] = $Source[$key]
        }
    }
}
```

#### Template Post-Actions

```json
{
  "actionId": "3A7C4B45-1F5D-4A30-960B-2576280946D8",
  "args": {
    "executable": "pwsh",
    "args": [
      "-File",
      "src/Enterprise.App/merge-config.ps1",
      "-Environment",
      "Production",
      "-EnableAuth",
      "$(EnableAuth)",
      "-EnableObservability",
      "$(EnableObservability)"
    ]
  }
}
```

#### Conditional File Inclusion

```json
{
  "condition": "(!EnableAuth)",
  "exclude": ["src/Enterprise.App/Config/auth*.json"]
}
```

## Result

This solution completely eliminates the repetitive manual configuration setup that was identified as a pain point. Users now get:

1. **Immediate Functionality** - No manual configuration required after template generation
2. **Clean Projects** - Only relevant configuration included
3. **Environment Optimization** - Appropriate settings for each environment
4. **Easy Maintenance** - Simple script-based regeneration when needed

The configuration management system transforms the enterprise template from a starting point requiring manual setup into a fully functional, production-ready solution out of the box.
