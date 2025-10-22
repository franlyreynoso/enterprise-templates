# Enterprise Templates Monorepo - AI Coding Instructions

## Repository Overview

This is a **monorepo of .NET enterprise templates** using a meta-template orchestration pattern. The repository contains three independent templates that can work standalone or together through orchestration.

### Core Templates

- **Web Template** (`templates/web-template/`) - Enterprise Blazor Server application with modular features
- **Backend Template** (`templates/backend-template/`) - Clean Architecture API with CQRS/MediatR
- **Fullstack Orchestrator** (`templates/fullstack-orchestrator/`) - Meta-template that coordinates both templates

### Key Principle: Template Independence

- Each template is **100% independent** and can be used standalone
- No cross-dependencies or coupling between templates
- Templates are coordinated at the workspace level, not internally modified
- Perfect integration achieved through matching patterns (Makefile, environment variables, Docker services)

## Repository Structure

```
enterprise-templates/
├── templates/
│   ├── web-template/           # Blazor Server template
│   │   ├── .template.config/   # Template configuration
│   │   ├── src/               # Application source
│   │   ├── Makefile           # Development commands
│   │   └── docker-compose.envs.yml
│   ├── backend-template/       # Clean Architecture API
│   │   ├── .template.config/   # Template configuration
│   │   ├── src/               # Application layers
│   │   ├── Makefile           # Development commands
│   │   └── docker-compose.envs.yml
│   └── fullstack-orchestrator/ # Coordination template
│       ├── .template.config/   # Orchestration parameters
│       ├── Makefile           # Unified commands
│       └── setup.ps1          # Automated setup
├── docs/                      # Architecture documentation
│   ├── ARCHITECTURE.md        # Orchestration design
│   ├── BACKEND-TEMPLATE.md    # Backend template details
│   ├── FULLSTACK_SETUP_GUIDE.md
│   ├── INSTALLATION_TROUBLESHOOTING.md
│   └── NAMING_CONVENTIONS.md
├── install-all.ps1            # Install all templates
├── uninstall-all.ps1          # Uninstall all templates
└── validate-templates.ps1     # Repository validation
```

## Development Patterns

### Template Installation (CRITICAL)

**⚠️ NEVER run `dotnet new install .` from repository root!**

Always use the provided scripts:

```bash
# Install all templates
.\install-all.ps1

# Reinstall (if already installed)
.\install-all.ps1 -Force

# Uninstall all templates
.\uninstall-all.ps1

# Or install individually from template directories
cd templates/web-template && dotnet new install .
cd templates/backend-template && dotnet new install .
cd templates/fullstack-orchestrator && dotnet new install .
```

**Why**: Running `dotnet new install .` from root causes duplicate registrations and path conflicts.

### Template Generation

```bash
# Individual templates
dotnet new blazor-enterprise -n MyCompany.Web
dotnet new enterprise-clean -n MyCompany.API

# Full-stack orchestration
dotnet new enterprise-fullstack -n MyCompany.Solution
```

### Development Workflow

Templates use environment-based development with Docker profiles:

```bash
# Environment-specific commands
make up ENV=dev              # Start development environment
make up ENV=staging          # Start staging environment
make up ENV=prod            # Start production environment
make down ENV=dev           # Stop environment

# Quick shortcuts
make up-dev                 # Start development
make up-staging            # Start staging
make up-prod               # Start production
```

For fullstack orchestrator:

```bash
make up-fullstack           # Start Web + Backend + Database
make up-web-only           # Start web template only
make up-backend-only       # Start backend template only
make logs                  # View all logs
make down                  # Stop everything
```

## Code Conventions

### PowerShell Scripts

- All scripts use `pwsh` (PowerShell Core 7+)
- Shebang: `#!/usr/bin/env pwsh`
- Error handling with `try/catch` blocks
- Colored output for user feedback
- Parameter validation with descriptive help

### Makefile Architecture

Both templates use **identical Makefile patterns**:

```makefile
# PowerShell detection for Windows
ifeq ($(OS),Windows_NT)
SHELL := pwsh.exe
.SHELLFLAGS := -NoProfile -Command
COMPOSE_FILE := docker-compose.envs.yml

# COMPOSE_PROFILES for environment management
$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d
```

### Environment Variables (Matching Pattern)

Backend and Web templates use **identical variable naming**:

```bash
# Database configuration
DB_USER=app
DB_PASS=app
POSTGRES_DB=enterprisetemplate_dev

# Service ports
JAEGER_UI_PORT=16686
SEQ_UI_PORT=5341
POSTGRES_PORT=5432
```

**Note**: Use `*_UI_PORT` suffix, not `*_WEB_PORT` (validated by validation script)

### Docker Service Names (Shared Infrastructure)

Templates use identical service names for sharing:

- `postgres` - PostgreSQL database
- `redis` - Redis cache
- `rabbitmq` - RabbitMQ message queue
- `jaeger` - Distributed tracing
- `seq` - Structured logging

### Port Coordination

Application ports are coordinated to avoid conflicts:

| Service     | Backend   | Web       | Purpose         |
|-------------|-----------|-----------|-----------------|
| Application | 5000-5002 | 3000-3002 | Avoid conflicts |
| PostgreSQL  | 5432-5434 | Same      | Shared service  |
| Redis       | 6379-6381 | Same      | Shared service  |

## Validation & Testing

### Repository Validation

Always run validation before committing:

```bash
pwsh ./validate-templates.ps1
```

This checks for:
- Backup files (*.backup, *.bak)
- Environment variable consistency
- Makefile corruption
- Documentation references
- Template.json validity
- .gitignore configuration

### Template Testing

```bash
# Test template installation
.\install-all.ps1

# Generate test projects
dotnet new enterprise-fullstack -n TestProject -o /tmp/test-output

# Validate generated structure
cd /tmp/test-output
make up-fullstack
make down
```

## Important Files

### Repository Level

- **README.md** - Main repository documentation
- **install-all.ps1** - Template installation script
- **validate-templates.ps1** - Repository health checks
- **docs/ARCHITECTURE.md** - Orchestration architecture
- **docs/NAMING_CONVENTIONS.md** - Naming rules and patterns

### Template Level

Each template has:
- **.template.config/template.json** - Template definition and parameters
- **Makefile** - Development commands
- **docker-compose.envs.yml** - Infrastructure with profiles
- **.env.dev/.env.staging/.env.prod** - Environment-specific configuration
- **setup.ps1** (web/orchestrator) - Post-generation setup

### Web Template Specific

- **src/Enterprise.App/Program.cs** - Conditional feature registration with `#if` directives
- **Directory.Packages.props** - Centralized package version management
- **Config/*.json** - Modular configuration fragments
- **merge-config.ps1** - Configuration assembly automation

### Backend Template Specific

- **src/API/Program.cs** - Minimal API setup
- **src/Application/** - CQRS handlers with MediatR
- **src/Domain/** - Clean Architecture domain layer
- **src/Infrastructure/** - External concerns

## Configuration Patterns

### Web Template: Modular Configuration

Uses configuration fragments instead of monolithic appsettings.json:

- Fragments in `src/Enterprise.App/Config/` (auth.json, http.json, features.json)
- PowerShell merge script combines fragments based on enabled features
- Environment-specific overrides (e.g., `auth.Development.json`)

### Backend Template: Traditional Configuration

Standard appsettings.json approach:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides

## Orchestration Philosophy

### Meta-Template Coordination

The fullstack orchestrator achieves integration **without modifying templates**:

1. **Workspace Structure**: Creates directory structure for both templates
2. **Shared Infrastructure**: Provides docker-compose with profiles for both
3. **Unified Commands**: Single Makefile for coordinated workflows
4. **Environment Management**: Coordinates configuration across templates

### Perfect Backend Alignment

Web template matches backend patterns exactly:
- Identical Makefile structure and commands
- Matching environment variable naming
- Same Docker service names and profiles
- Coordinated port strategies

## Common Workflows

### Adding a New Template

1. Create template in `templates/new-template/`
2. Add `.template.config/template.json` with unique `shortName`
3. Create Makefile following existing patterns
4. Add docker-compose.envs.yml if infrastructure needed
5. Update `install-all.ps1` and `uninstall-all.ps1`
6. Document in README.md and docs/

### Updating Template Patterns

When updating Makefile or environment patterns:

1. Update backend template first (reference implementation)
2. Mirror changes in web template for consistency
3. Update orchestrator if coordination needed
4. Run `validate-templates.ps1` to verify consistency
5. Update documentation

### Troubleshooting Template Issues

Common issues and solutions:

1. **Duplicate template registrations**:
   - Run `.\uninstall-all.ps1`
   - Run `.\install-all.ps1`

2. **Path conflicts**:
   - Never use `dotnet new install .` from root
   - Use provided installation scripts

3. **Port conflicts**:
   - Check environment-specific .env files
   - Ensure unique ports per environment

4. **Missing infrastructure**:
   - Verify Docker is running
   - Check COMPOSE_PROFILES in Makefile
   - Review docker-compose.envs.yml

## Security & Best Practices

### Secrets Management

- Never commit secrets to repository
- Use environment variables for sensitive data
- Template defaults should be non-sensitive placeholders
- Document secret requirements in template READMEs

### Template Security

- Validate user inputs in setup scripts
- Use parameterized Docker commands (avoid shell injection)
- Include security headers in web applications
- Document security configuration in generated projects

### Git Practices

- .gitignore excludes backup files (*.backup, *.bak)
- Templates should generate appropriate .gitignore for projects
- Exclude build artifacts (bin/, obj/, node_modules/)
- Include .env.example but exclude .env files

## Documentation Guidelines

### Template Documentation

Each template should document:
- Architecture and structure
- Configuration options
- Development commands
- Testing strategy
- Deployment guidance

### Repository Documentation

Keep updated:
- README.md - Overview and quick start
- docs/ARCHITECTURE.md - Design decisions
- Template-specific guides in docs/
- CHANGELOG for version history

## Testing Approach

### Manual Testing

1. Install templates: `.\install-all.ps1`
2. Generate test project in `/tmp`: `dotnet new enterprise-fullstack -n Test -o /tmp/test`
3. Verify structure and files
4. Test development commands: `make up-fullstack`
5. Verify services are accessible
6. Clean up: `make down` and remove `/tmp/test`

### Validation Testing

Run before any commit:
```bash
pwsh ./validate-templates.ps1
```

### Integration Testing

Test orchestrator coordination:
1. Generate fullstack project
2. Start with `make up-fullstack`
3. Verify web (3000) and API (5000) are running
4. Verify shared database connectivity
5. Check distributed tracing in Jaeger (16686)
6. Review logs in Seq (5341)

## Key Reminders

- **Template Independence**: Never create dependencies between templates
- **Pattern Matching**: Keep Makefile and environment patterns identical
- **Validation First**: Run `validate-templates.ps1` before commits
- **Installation Scripts**: Always use provided installation scripts
- **Documentation**: Update docs when changing patterns or structure
- **Environment Profiles**: Use COMPOSE_PROFILES for environment management
- **Shared Services**: Coordinate service names and ports for integration

## References

- [Architecture Guide](../docs/ARCHITECTURE.md) - Orchestration system design
- [Backend Template](../docs/BACKEND-TEMPLATE.md) - API template details
- [Fullstack Setup Guide](../docs/FULLSTACK_SETUP_GUIDE.md) - Step-by-step setup
- [Installation Troubleshooting](../docs/INSTALLATION_TROUBLESHOOTING.md) - Common issues
- [Naming Conventions](../docs/NAMING_CONVENTIONS.md) - Project naming rules
