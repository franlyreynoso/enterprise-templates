# Enterprise UI Template - Makefile# Enterprise UI Template - Unified Development Commands

# Matches backend template patterns exactly for perfect integration# Provides consistent command interface across platforms matching backend template



# --- Windows (PowerShell) vs Unix (sh) detection ---# Detect shell environment for cross-platform compatibility

ifeq ($(OS),Windows_NT)ifeq ($(OS),Windows_NT)

SHELL := pwsh.exe    DETECTED_OS := Windows

.SHELLFLAGS := -NoProfile -Command    SHELL_TYPE := $(shell echo $$0 2>nul || echo cmd)

COMPOSE_FILE := docker-compose.envs.yml    PWD := $(shell cd)

ENV ?= develse

    DETECTED_OS := Unix

check-compose:    SHELL_TYPE := $(SHELL)

	@if (-not (Get-Command docker -ErrorAction SilentlyContinue)) { Write-Error '❌ Docker CLI not found.'; exit 1 }; if (-not ((docker compose version) 2>$$null)) { Write-Error '❌ `docker compose` not available. Enable Compose V2 in Docker Desktop or install the plugin.'; exit 1 }    PWD := $(shell pwd)

endif

up-dev: check-compose

	$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60# Set PowerShell preference on Windows (matching backend template)

ifeq ($(DETECTED_OS),Windows)

down-dev: check-compose    ifneq ($(findstring pwsh,$(SHELL_TYPE)),)

	$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) down -v --remove-orphans        SHELL := pwsh.exe -c

    else ifneq ($(findstring powershell,$(SHELL_TYPE)),)

up-staging: check-compose        SHELL := powershell.exe -Command

	$$env:COMPOSE_PROFILES='staging'; docker compose --env-file .env.staging -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60    else

        SHELL := pwsh.exe -c

down-staging: check-compose    endif

	$$env:COMPOSE_PROFILES='staging'; docker compose --env-file .env.staging -f $(COMPOSE_FILE) down -v --remove-orphansendif



up-prod: check-compose# Default environment

	$$env:COMPOSE_PROFILES='prod'; docker compose --env-file .env.prod -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60ENV ?= dev



down-prod: check-compose# Environment validation

	$$env:COMPOSE_PROFILES='prod'; docker compose --env-file .env.prod -f $(COMPOSE_FILE) down -v --remove-orphansVALID_ENVS := dev staging prod

ifeq ($(filter $(ENV),$(VALID_ENVS)),)

up-integrated: check-compose    $(error Invalid ENV=$(ENV). Must be one of: $(VALID_ENVS))

	$$env:COMPOSE_PROFILES='integrated'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d --wait --wait-timeout 120endif



down-integrated: check-compose# Environment-specific configurations (matching backend pattern)

	$$env:COMPOSE_PROFILES='integrated'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) down -v --remove-orphansENV_FILE := .env.$(ENV)

COMPOSE_FILE := docker-compose.envs.yml

# Environment-specific shortcuts (matching backend template)COMPOSE_PROFILES := $(ENV)

up: ENV ?= dev

up: check-compose# Export environment variables for docker-compose

	$$env:COMPOSE_PROFILES='$(ENV)'; docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60export COMPOSE_PROFILES



down: ENV ?= dev# Verify required files exist

down: check-composeifeq ($(wildcard $(ENV_FILE)),)

	$$env:COMPOSE_PROFILES='$(ENV)'; docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) down -v --remove-orphans    $(error Environment file $(ENV_FILE) not found)

endif

logs: check-composeifeq ($(wildcard $(COMPOSE_FILE)),)

	$$env:COMPOSE_PROFILES='$(ENV)'; docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) logs -f    $(error Compose file $(COMPOSE_FILE) not found)

endif

logs-seq: check-compose

	$$env:COMPOSE_PROFILES='$(ENV)'; docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) logs -f seq.PHONY: help up down logs build clean test config up-dev up-staging up-prod up-integrated logs-seq logs-otel logs-integrated test-e2e



logs-integrated: check-compose# Display help information

	$$env:COMPOSE_PROFILES='integrated'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) logs -fhelp:

	@echo "Enterprise UI Template - Development Commands"

config: ENV ?= dev	@echo ""

config:	@echo "Environment Commands (ENV=dev|staging|prod):"

	@Write-Host "🔧 Generating configuration for environment: $(ENV)" -ForegroundColor Yellow	@echo "  make up ENV=dev         Start development environment"

	cd src/Enterprise.App && .\merge-config.ps1 -Environment $(ENV) -EnableAuth -EnableObservability	@echo "  make down ENV=dev       Stop development environment"

	@echo "  make logs ENV=dev       View environment logs"

test:	@echo ""

	dotnet test	@echo "Quick Commands:"

	@echo "  make up-dev             Start development (shortcut)"

test-e2e:	@echo "  make up-staging         Start staging (shortcut)"

	@Write-Host "🧪 Running end-to-end tests..." -ForegroundColor Blue	@echo "  make up-prod           Start production (shortcut)"

	dotnet test tests/ --filter "Category=E2E"	@echo "  make up-integrated      Start full-stack (UI + Backend + DB)"

	@echo ""

clean:	@echo "Observability:"

	dotnet clean	@echo "  make logs-seq          View centralized logging (Seq)"

	docker system prune -f	@echo "  make logs-otel         View distributed tracing (Jaeger via OpenTelemetry)"

	@echo "  make logs-integrated   View full-stack logs"

help:	@echo ""

	@Write-Host "📋 Enterprise UI Template Commands" -ForegroundColor Cyan	@echo "Testing & Configuration:"

	@Write-Host ""	@echo "  make test              Run unit tests"

	@Write-Host "🚀 Main Commands:" -ForegroundColor Green	@echo "  make test-e2e          Run end-to-end tests"

	@Write-Host "  make up ENV=dev          - Start development environment"	@echo "  make config ENV=dev    Generate configuration"

	@Write-Host "  make up ENV=staging      - Start staging environment"	@echo ""

	@Write-Host "  make up ENV=prod         - Start production environment"	@echo "Cleanup:"

	@Write-Host "  make up-integrated       - Start with backend integration"	@echo "  make clean             Clean build artifacts and containers"

	@Write-Host "  make down ENV=dev        - Stop environment"

	@Write-Host ""# Start environment with specific configuration (matching backend pattern)

	@Write-Host "🔧 Configuration:" -ForegroundColor Yellowup:

	@Write-Host "  make config ENV=dev      - Generate environment config"	@echo "Starting $(ENV) environment..."

	@Write-Host ""	@echo "Environment file: $(ENV_FILE)"

	@Write-Host "📝 Monitoring:" -ForegroundColor Magenta	@echo "Compose file: $(COMPOSE_FILE)"

	@Write-Host "  make logs ENV=dev        - View logs"	@echo "Compose profiles: $(COMPOSE_PROFILES)"

	@Write-Host "  make logs-seq           - View centralized logs"	docker-compose --env-file $(ENV_FILE) -f $(COMPOSE_FILE) up -d

	@Write-Host ""	@echo "$(ENV) environment started successfully!"

	@Write-Host "🧪 Testing:" -ForegroundColor Blue	@echo ""

	@Write-Host "  make test              - Run unit tests"	@echo "Services:"

	@Write-Host "  make test-e2e          - Run end-to-end tests"	@echo "  - Blazor UI: https://localhost:7001"

	@echo "  - Health: https://localhost:7001/health"

elseifeq ($(ENV),dev)

# Unix version (sh) - matching backend template structure	@echo "  - pgAdmin: http://localhost:5050"

SHELL := /bin/shendif

COMPOSE_FILE := docker-compose.envs.yml	@echo "  - Seq Logs: http://localhost:5341"

ENV ?= dev	@echo "  - Jaeger Tracing: http://localhost:16686"



check-compose:# Stop environment

	@command -v docker >/dev/null 2>&1 || { echo "❌ Docker CLI not found." >&2; exit 1; }down:

	@docker compose version >/dev/null 2>&1 || { echo "❌ 'docker compose' not available." >&2; exit 1; }	@echo "Stopping $(ENV) environment..."

	docker-compose --env-file $(ENV_FILE) -f $(COMPOSE_FILE) down

up-dev: check-compose	@echo "$(ENV) environment stopped!"

	COMPOSE_PROFILES=dev docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

# View environment logs

down-dev: check-composelogs:

	COMPOSE_PROFILES=dev docker compose --env-file .env.dev -f $(COMPOSE_FILE) down -v --remove-orphans	@echo "Viewing $(ENV) environment logs..."

	docker-compose --env-file $(ENV_FILE) -f $(COMPOSE_FILE) logs -f

up-staging: check-compose

	COMPOSE_PROFILES=staging docker compose --env-file .env.staging -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60# Build application and containers

build:

down-staging: check-compose	@echo "Building $(ENV) application..."

	COMPOSE_PROFILES=staging docker compose --env-file .env.staging -f $(COMPOSE_FILE) down -v --remove-orphans	docker-compose --env-file $(ENV_FILE) -f $(COMPOSE_FILE) build

	@echo "Build completed!"

up-prod: check-compose

	COMPOSE_PROFILES=prod docker compose --env-file .env.prod -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60# Clean up containers and build artifacts

clean:

down-prod: check-compose	@echo "Cleaning up containers and build artifacts..."

	COMPOSE_PROFILES=prod docker compose --env-file .env.prod -f $(COMPOSE_FILE) down -v --remove-orphans	docker-compose -f $(COMPOSE_FILE) down --remove-orphans --volumes

	docker system prune -f

up: check-compose	dotnet clean

	COMPOSE_PROFILES=$(ENV) docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60	@echo "Cleanup completed!"



down: check-compose# Run unit tests

	COMPOSE_PROFILES=$(ENV) docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) down -v --remove-orphanstest:

	@echo "Running unit tests..."

logs: check-compose	dotnet test

	COMPOSE_PROFILES=$(ENV) docker compose --env-file .env.$(ENV) -f $(COMPOSE_FILE) logs -f

# Run end-to-end tests (auto-installs Playwright)

config:test-e2e:

	@echo "🔧 Generating configuration for environment: $(ENV)"	@echo "Running end-to-end tests..."

	cd src/Enterprise.App && pwsh ./merge-config.ps1 -Environment $(ENV) -EnableAuth -EnableObservability	@echo "Installing Playwright browsers if needed..."

ifeq ($(DETECTED_OS),Windows)

test:	pwsh -Command "dotnet build tests/Enterprise.Ui.Tests; cd tests/Enterprise.Ui.Tests; dotnet exec playwright.ps1 install"

	dotnet testelse

	pwsh -c "dotnet build tests/Enterprise.Ui.Tests; cd tests/Enterprise.Ui.Tests; dotnet exec playwright.ps1 install"

clean:endif

	dotnet clean	dotnet test tests/Enterprise.Ui.Tests --filter "Category=E2E"

	docker system prune -f

# Generate configuration for environment

help:config:

	@echo "📋 Enterprise UI Template Commands"	@echo "Generating $(ENV) configuration..."

	@echo "  make up ENV=dev    - Start development environment"  ifeq ($(DETECTED_OS),Windows)

	@echo "  make down ENV=dev  - Stop environment"	cd src/Enterprise.App && pwsh -File merge-config.ps1 -Environment $(ENV) -EnableAuth -EnableObservability

	@echo "  make logs ENV=dev  - View logs"else

	@echo "  make test         - Run tests"	cd src/Enterprise.App && pwsh merge-config.ps1 -Environment $(ENV) -EnableAuth -EnableObservability

endif

endif	@echo "Configuration generated for $(ENV)!"



.PHONY: up-dev up-staging up-prod up-integrated down-dev down-staging down-prod down-integrated up down logs logs-seq logs-integrated config test test-e2e clean help check-compose# Quick environment shortcuts
up-dev:
	$(MAKE) up ENV=dev

up-staging:
	$(MAKE) up ENV=staging

up-prod:
	$(MAKE) up ENV=prod

# Integrated full-stack environment
up-integrated:
	@echo "Starting integrated full-stack environment..."
	@echo "Note: Requires backend template running simultaneously"
	COMPOSE_PROFILES=integrated docker-compose --env-file .env.dev -f $(COMPOSE_FILE) up -d
	@echo "Integrated environment started!"
	@echo ""
	@echo "Full Stack Services:"
	@echo "  - UI: https://localhost:7001"
	@echo "  - Backend API: https://localhost:7000 (from backend template)"
	@echo "  - Database: localhost:5432"
	@echo "  - pgAdmin: http://localhost:5050"
	@echo "  - Seq Logs: http://localhost:5341"
	@echo "  - Jaeger Tracing: http://localhost:16686"

# Observability log shortcuts (matching backend naming)
logs-seq:
	@echo "Opening Seq logs..."
	@echo "URL: http://localhost:5341"

logs-otel:
	@echo "Opening OpenTelemetry tracing (Jaeger UI)..."
	@echo "URL: http://localhost:16686"

logs-integrated:
	@echo "Viewing integrated full-stack logs..."
	COMPOSE_PROFILES=integrated docker-compose --env-file .env.dev -f $(COMPOSE_FILE) logs -f