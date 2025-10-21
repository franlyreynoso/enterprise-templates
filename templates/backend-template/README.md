# Enterprise Template Repository

This repository contains the source code and configuration for the **Enterprise Clean Architecture Template** - a comprehensive .NET solution template that implements Clean Architecture with enterprise-grade cross-cutting concerns.

## 🎯 What This Repository Contains

This is the **template source repository**. When you install and use this template, it generates new projects based on the code and configuration stored here.

- **Template Source Code**: The actual .NET solution that serves as the template
- **Template Configuration**: Settings that control how the template behaves when instantiated
- **Documentation**: Architecture guides and usage instructions

## 📦 Installing the Template

To use this template to create new projects:

```bash
# Install the template from this repository
dotnet new install .

# Create a new project from the template
dotnet new enterprise-clean -n MyCompany.MyProject

# See all available options
dotnet new enterprise-clean --help
```

## 🔧 Template Development

### Repository Structure

```
├── src/                          # Template source code
│   ├── Api/                      # Web API project
│   ├── Application/              # Application layer
│   ├── Domain/                   # Domain layer
│   ├── Infrastructure/           # Infrastructure layer
│   └── Worker/                   # Background worker
├── docs/                         # Documentation included in generated projects
├── .template.config/             # Template configuration
│   └── template.json            # Template metadata and options
├── TEMPLATE_README.md           # README that gets renamed in generated projects
└── .gitignore                   # Standard .NET gitignore
```

### Making Changes to the Template

1. **Modify Source Code**: Make changes to the projects in `src/`
2. **Update Template Options**: Edit `.template.config/template.json`
3. **Test the Template**:

   ```bash
   # Reinstall the template
   dotnet new uninstall .
   dotnet new install .

   # Test generation
   dotnet new enterprise-clean -n TestProject
   ```

### Template Configuration

The `.template.config/template.json` file controls:

- **Template metadata** (name, description, tags)
- **Template parameters** (cloud provider, database, messaging, etc.)
- **File exclusions** (what gets excluded from generated projects)
- **File renames** (e.g., `TEMPLATE_README.md` → `README.md`)
- **Conditional file inclusion** based on selected options

### Repository vs Generated Project Files

**Repository Files (excluded from template generation):**

- `.git/` - This repository's Git history
- `.template.config/` - Template configuration
- `README.md` (this file) - Repository documentation

**Generated Project Files:**

- `TEMPLATE_README.md` → `README.md` - Project-specific documentation
- All source code with parameter substitutions applied
- `docs/` folder with architecture documentation

## 🚀 Template Features

The generated projects include:

### Core Architecture

- **Clean Architecture** with proper layer separation
- **CQRS + MediatR** for command/query handling
- **Domain-Driven Design** patterns

### Cross-Cutting Concerns

- **Security**: JWT authentication, HTTPS enforcement
- **Caching**: Redis distributed caching (optional)
- **Health Checks**: Comprehensive monitoring endpoints
- **Logging**: Structured logging with Serilog + Seq
- **Resilience**: Polly patterns (retry, circuit breaker, timeout)
- **Messaging**: MassTransit with multiple transport options
- **Validation**: FluentValidation integration

### Infrastructure Options

- **Cloud**: Azure or On-Premises configurations
- **Database**: PostgreSQL or SQL Server
- **Message Bus**: RabbitMQ, Azure Service Bus, or None
- **Multi-tenancy**: Optional tenant isolation patterns

## 📋 Template Parameters

| Parameter     | Description          | Options                               | Default    |
| ------------- | -------------------- | ------------------------------------- | ---------- |
| `cloud`       | Deployment target    | `Azure`, `OnPrem`                     | `OnPrem`   |
| `db`          | Database provider    | `SqlServer`, `Postgres`               | `Postgres` |
| `bus`         | Message transport    | `AzureServiceBus`, `RabbitMQ`, `None` | `RabbitMQ` |
| `multitenant` | Multi-tenant support | `true`, `false`                       | `false`    |
| `caching`     | Redis caching        | `true`, `false`                       | `true`     |
| `monitoring`  | Health checks        | `true`, `false`                       | `true`     |
| `resilience`  | Polly patterns       | `true`, `false`                       | `true`     |
| `logging`     | Logging solution     | `Seq`, `Console`                      | `Seq`      |

## 🤝 Contributing to the Template

1. **Fork this repository**
2. **Create a feature branch** for your changes
3. **Test your changes** by generating test projects
4. **Update documentation** if adding new features or parameters
5. **Submit a pull request** with a clear description of changes

### Testing Changes

Always test template changes before committing:

```bash
# Test with different parameter combinations
dotnet new enterprise-clean -n Test1 --cloud Azure --db SqlServer
dotnet new enterprise-clean -n Test2 --cloud OnPrem --bus None --caching false
dotnet new enterprise-clean -n Test3 --multitenant true --logging Console
```

## 📖 Documentation

- **[Architecture Guide](docs/ARCHITECTURE.md)** - Detailed architecture explanation
- **[Environment Setup](docs/ENVIRONMENTS.md)** - Development environment configuration
- **[References](docs/REFERENCES.md)** - External resources and libraries used

## 📄 License

This template is licensed under the MIT License - see the LICENSE file for details.

---

**Note**: This README is for the template repository. Generated projects will have their own README based on `TEMPLATE_README.md`.
