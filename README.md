# Enterprise Templates Monorepo

A comprehensive collection of .NET enterprise templates with perfect backend integration orchestration.

> **⚠️ Important:** Always use `.\install-all.ps1` to install templates. Do not run `dotnet new install .` from the repository root.

## 🚀 Architecture

This monorepo contains:

- **Web Template**: Enterprise Blazor Server template with backend integration capability
- **Backend Template**: Clean Architecture microservice with CQRS/MediatR
- **Fullstack Orchestrator**: Meta-template that coordinates both templates independently

## 📁 Repository Structure

```
enterprise-templates/
├── templates/
│   ├── web-template/                   # Enterprise Blazor Server template
│   ├── backend-template/              # Clean Architecture API template
│   └── fullstack-orchestrator/        # Meta-template for coordination
├── docs/
│   ├── ARCHITECTURE.md                # Orchestration architecture
│   └── BACKEND-TEMPLATE.md           # Backend template documentation
├── install-all.ps1                   # Install all templates
└── README.md                         # This file
```

## 🎯 Orchestration Approach

### Independent Templates

- Each template remains completely independent
- No cross-dependencies or coupling between templates
- Templates can be used individually or together

### Workspace-Level Coordination

- Meta-template creates workspace structure for both templates
- Shared infrastructure patterns without template modification
- Unified development commands for full-stack workflows

### Perfect Backend Alignment

- Web template matches backend patterns exactly:
  - Makefile with PowerShell detection and COMPOSE_PROFILES
  - Single docker-compose.envs.yml with profiles ["dev", "staging", "prod"]
  - Backend variable naming (DB_USER, DB_PASS, etc.)
  - Identical service names and port strategies

## 🔧 Usage

### Install Templates

```bash
# Install all templates
.\install-all.ps1

# Reinstall templates (if already installed)
.\install-all.ps1 -Force

# Uninstall all templates
.\uninstall-all.ps1

# Or install individually
cd templates/web-template && dotnet new install .
cd templates/backend-template && dotnet new install .
cd templates/fullstack-orchestrator && dotnet new install .
```

### Generate Projects

```bash
# Individual templates
dotnet new blazor-enterprise -n MyCompany.Web
dotnet new enterprise-clean -n MyCompany.API

# Full-stack orchestration
dotnet new enterprise-fullstack -n MyCompany.Solution
```

### Development Workflow

```bash
# In orchestrated workspace
make up-fullstack         # Start Web + Backend + Database
make up-web-only          # Start web template only
make up-backend-only     # Start backend template only
make logs                # View all logs
make down               # Stop everything
```

## 🔧 Troubleshooting

### Template Installation Issues

If you encounter errors like:
- "The following templates use the same identity"
- "Template package location is not supported, or doesn't exist"
- References to non-existent paths like `ui-template`

**Solution:**

```bash
# 1. Uninstall all existing templates
.\uninstall-all.ps1

# 2. Reinstall templates
.\install-all.ps1
```

**Important:** 
- ⚠️ **Do NOT run** `dotnet new install .` from the repository root
- ✅ **Always use** the `install-all.ps1` script to install templates
- ✅ Or install individual templates from their specific directories

### Why This Happens

The .NET template system recursively scans directories when you run `dotnet new install .` from the root. This can cause:
- Duplicate template registrations
- Path conflicts between case-sensitive and case-insensitive systems
- Templates being registered from multiple locations

The `install-all.ps1` script properly installs each template from its correct location and handles path normalization.

## 📖 Key Benefits

### 🏗️ Clean Separation

- Templates remain independent and reusable
- No coupling between web and backend templates
- Meta-template handles coordination without modification

### 🔄 Perfect Integration

- Service sharing works seamlessly (database, observability stack)
- Port conflicts resolved through orchestration
- Environment variables aligned across templates

### 🚀 Unified Experience

- Single command to start full-stack development
- Coordinated environment management
- Shared infrastructure without duplication

## 📚 Documentation

- **[Architecture Guide](docs/ARCHITECTURE.md)** - Orchestration system design
- **[Backend Template](docs/BACKEND-TEMPLATE.md)** - API template details

---

Built with ❤️ for enterprise development workflows.
