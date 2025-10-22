# Fullstack Orchestrator Setup Guide

This guide explains how to set up and run the Enterprise Fullstack Orchestrator template.

## Prerequisites

- .NET 9.0 SDK
- Docker Desktop
- PowerShell (for Windows) or pwsh (for Linux/Mac)

## Step-by-Step Setup

### 1. Install the Templates

First, install both the Web and API templates:

```bash
dotnet new install <path-to-web-template>
dotnet new install <path-to-backend-template>
```

Or if installing from NuGet:

```bash
dotnet new install Enterprise.Templates.Blazor
dotnet new install Enterprise.Templates.CleanArchitecture
```

### 2. Generate the Fullstack Workspace

Create your fullstack solution:

```bash
dotnet new enterprise-fullstack -n MyCompany.Fullstack
cd MyCompany.Fullstack
```

### 3. Start the Development Environment

The environment setup will automatically generate Web and API projects on first run:

```bash
make up-fullstack
```

This command will:
1. Check if Web and API projects exist
2. If they don't exist, automatically run `make setup` to generate them
3. Update `.env` files with the generated project names
4. Start the full development stack

Alternatively, you can manually generate projects first with custom options:

```bash
# Generate projects with specific names and features
./setup.ps1 -WebProjectName "MyWeb" -APIProjectName "MyApi" -Cloud "Azure" -Database "Postgres"
```

This will start:
- Web Application (Blazor)
- API Application (Clean Architecture)
- PostgreSQL database
- Redis cache
- RabbitMQ message broker
- Jaeger tracing
- Seq logging
- pgAdmin (database management)
- MailHog (email testing)

### 4. Access Your Applications

- **Web Application**: http://localhost:3000
- **API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger
- **Database Admin (pgAdmin)**: http://localhost:5050 (admin@example.com / admin)
- **Message Queue (RabbitMQ)**: http://localhost:15672 (app / app)
- **Distributed Tracing (Jaeger)**: http://localhost:16686
- **Centralized Logging (Seq)**: http://localhost:5341

## Common Issues

### Projects Generated Automatically

When you run `make up-fullstack` for the first time, it will automatically generate the Web and API projects using default names (`WEB_PROJECT_NAME` and `API_PROJECT_NAME`). If you want custom names, run the setup script manually first:

```bash
./setup.ps1 -WebProjectName "MyCustomWeb" -APIProjectName "MyCustomApi"
```

### "Project not found" Error (Legacy)

If you see an error like:
```
❌ Web project 'WEB_PROJECT_NAME' not found!
```

This means you haven't updated the `.env.dev` file with your actual project names. Make sure:
1. You've generated both Web and API projects
2. You've updated `WEB_PROJECT` and `API_PROJECT` in `.env.dev` to match your actual project directory names

### Solution File Not Found Error

If you see an error like:
```
"/enterprise-ui-template-full.sln": not found
```

This was a bug in older versions that has been fixed. Make sure you're using the latest version of the templates.

## Additional Commands

```bash
# Start only the web application
make up-web

# Start only the API application
make up-api

# View logs
make logs

# Check health of all services
make health

# Stop all services
make down

# Clean build outputs
make clean
```

## Environment Management

You can run the stack in different environments:

```bash
# Development (default)
make up-fullstack ENV=dev

# Staging
make up-fullstack ENV=staging

# Production simulation
make up-fullstack ENV=prod
```

Each environment uses different ports and configuration to avoid conflicts.
