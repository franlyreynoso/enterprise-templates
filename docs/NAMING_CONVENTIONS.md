# Project and Solution Naming Conventions

This document describes how generated solutions and projects are named according to input parameters in the enterprise templates.

## Overview

All templates in this repository follow consistent naming conventions that ensure generated artifacts match the input parameters provided during template instantiation.

## Web Template (blazor-enterprise)

### Solution Naming

The solution file is automatically named based on the `-n` parameter:

```bash
dotnet new blazor-enterprise -n MyCompany.MyApp
# Generates: MyCompany.MyApp.sln
```

### Project Naming

#### Main Application Project
The main Blazor Server application project is named exactly as the `-n` parameter:

```bash
# Input
dotnet new blazor-enterprise -n Acme.WebPortal

# Generated Structure
Acme.WebPortal/
├── Acme.WebPortal.sln           # Solution file
└── src/
    └── Acme.WebPortal/          # Main application project
        └── Acme.WebPortal.csproj
```

#### Supporting Library Projects
Supporting library projects maintain their `Enterprise.Ui.*` naming to indicate they are reusable libraries:

- `Enterprise.Ui.Shared` - Shared UI components and layouts
- `Enterprise.Ui.Auth` - Authentication and authorization (if enabled)
- `Enterprise.Ui.Http` - HTTP resilience patterns (if enabled)
- `Enterprise.Ui.ApiClient` - API client generation (if enabled)
- `Enterprise.Ui.Observability` - OpenTelemetry observability (if enabled)
- `Enterprise.Ui.FeatureFlags` - Feature flag management (if enabled)
- `Enterprise.Ui.Security` - Security headers (if enabled)

#### Test Projects
Test projects use the `Enterprise.Ui.Tests` naming pattern to indicate they test the UI library components:

- `Enterprise.Ui.Tests` - bUnit and Playwright tests (if enabled)

### Template Parameter: `sourceName`

The web template uses `sourceName: "Enterprise.App"` in `template.json`, which means:

- The template engine replaces all occurrences of "Enterprise.App" with the value provided via `-n`
- This applies to:
  - Solution file name
  - Main project folder name
  - Main project file (.csproj) name
  - References in the solution file

## Backend Template (enterprise-clean)

### Solution Naming

The solution file is automatically named based on the `-n` parameter:

```bash
dotnet new enterprise-clean -n MyCompany.API
# Generates: MyCompany.API.sln
```

### Project Naming

All projects in the backend template follow the Clean Architecture pattern and are prefixed with the provided name:

```bash
# Input
dotnet new enterprise-clean -n Contoso.OrderService

# Generated Structure
Contoso.OrderService/
├── Contoso.OrderService.sln
└── src/
    ├── Api/
    │   └── Contoso.OrderService.Api.csproj
    ├── Application/
    │   └── Contoso.OrderService.Application.csproj
    ├── Domain/
    │   └── Contoso.OrderService.Domain.csproj
    ├── Infrastructure/
    │   └── Contoso.OrderService.Infrastructure.csproj
    └── Worker/
        └── Contoso.OrderService.Worker.csproj
```

### Template Parameter: `sourceName`

The backend template uses `sourceName: "EnterpriseTemplate"` in `template.json`, which means:

- All occurrences of "EnterpriseTemplate" are replaced with the value provided via `-n`
- This applies to all project names, namespaces, and references

## Fullstack Orchestrator Template (enterprise-fullstack)

The fullstack orchestrator is a meta-template that doesn't generate solution or project files directly. Instead, it:

1. Creates a workspace structure
2. Provides Docker Compose configuration for coordinating web and backend templates
3. Uses customizable project names via parameters:
   - `webProjectName` (default: "Web")
   - `apiProjectName` (default: "API")

```bash
dotnet new enterprise-fullstack -n MyCompany.Platform --webProjectName Portal --apiProjectName Services
```

## Naming Best Practices

### Recommended Naming Patterns

1. **Company.Product** - For simple applications
   ```bash
   dotnet new blazor-enterprise -n Acme.CustomerPortal
   ```

2. **Company.Product.Component** - For modular systems
   ```bash
   dotnet new enterprise-clean -n Contoso.ECommerce.OrderService
   ```

3. **Organization.Division.Product** - For large enterprises
   ```bash
   dotnet new blazor-enterprise -n GlobalCorp.Retail.InventoryApp
   ```

### Naming Conventions

- Use **PascalCase** for all names
- Use **dots (.)** to separate namespace segments
- Avoid underscores, hyphens, or spaces
- Keep names descriptive but concise
- Follow your organization's namespace conventions

## Template Configuration

### How It Works

The .NET template engine uses the `sourceName` parameter in `template.json`:

```json
{
  "sourceName": "Enterprise.App",
  "preferNameDirectory": true
}
```

When you run:
```bash
dotnet new blazor-enterprise -n MyCompany.MyApp
```

The template engine:
1. Creates a directory named `MyCompany.MyApp`
2. Replaces all occurrences of "Enterprise.App" with "MyCompany.MyApp"
3. Renames files containing "Enterprise.App" in their names
4. Updates all references in project files and solution files

### Solution File Naming

Solution files are automatically renamed to match the output name:

- **Web Template**: `Enterprise.App.sln` → `{YourName}.sln`
- **Backend Template**: `EnterpriseTemplate.sln` → `{YourName}.sln`

This is handled automatically by the template engine's default behavior.

## Troubleshooting

### Issue: Solution file has incorrect name

**Problem**: Generated solution file doesn't match the expected name.

**Solution**: 
- Ensure you're using the latest version of the templates
- Run `dotnet new uninstall` to remove old versions
- Reinstall using the installation scripts

### Issue: Main application project is missing

**Problem**: Solution references a project that doesn't exist.

**Solution**: 
- Verify you're using the updated templates that include the Enterprise.App project
- The Enterprise.App folder should exist in the template source

### Issue: Project references are broken

**Problem**: Visual Studio or build errors about missing projects.

**Solution**: 
- Run `dotnet restore` in the solution directory
- Verify all referenced projects exist in the src/ directory
- Check that the solution file includes all necessary projects

## Migration Notes

### Updating from Previous Versions

If you have projects generated from older versions of these templates:

1. **Solution File**: Rename your `.sln` file to match your project name
2. **Main Project**: Ensure your main application project folder exists and is properly named
3. **References**: Update all project references in the solution file to use the correct names

### Example Migration

Before:
```
MyProject/
├── Company.SourceName.sln    # Incorrect literal name
└── src/
    └── (missing main app folder)
```

After:
```
MyProject/
├── MyCompany.MyApp.sln        # Correct parameterized name
└── src/
    └── MyCompany.MyApp/       # Main app folder exists
        └── MyCompany.MyApp.csproj
```

## Summary

| Template | Solution Name | Main Project Name | Library Projects |
|----------|--------------|-------------------|------------------|
| Web Template | `{YourName}.sln` | `{YourName}` | `Enterprise.Ui.*` |
| Backend Template | `{YourName}.sln` | `{YourName}.*` (5 projects) | N/A |
| Fullstack Orchestrator | N/A | Configurable | N/A |

All naming is controlled by the `-n` parameter you provide to `dotnet new`, ensuring consistency across all generated artifacts.
