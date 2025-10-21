# Enterprise Templates Monorepo

A comprehensive collection of .NET enterprise templates with perfect backend integration orchestration.

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

- UI template matches backend patterns exactly:
  - Makefile with PowerShell detection and COMPOSE_PROFILES
  - Single docker-compose.envs.yml with profiles ["dev", "staging", "prod"]
  - Backend variable naming (DB_USER, DB_PASS, etc.)
  - Identical service names and port strategies

## 🔧 Usage

### Install Templates

```bash
# Install all templates
.\install-all.ps1

# Or install individually
cd templates/web-template && dotnet new install .
cd templates/backend-template && dotnet new install .
cd templates/fullstack-orchestrator && dotnet new install .
```

### Generate Projects

```bash
# Individual templates
dotnet new blazor-enterprise -n MyCompany.UI
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
