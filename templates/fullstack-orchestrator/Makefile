# Enterprise Fullstack Orchestrator - Makefile
# Unified commands for coordinated Web and API development
ENV ?= dev
WEB_PROJECT := WEB_PROJECT_NAME
API_PROJECT := API_PROJECT_NAME

help:
	@powershell -NoProfile -Command "Write-Host '🚀 Enterprise Fullstack Orchestrator' -ForegroundColor Green; Write-Host '================================'; Write-Host 'Environment Commands:' -ForegroundColor Cyan; Write-Host '  make up-fullstack     Start complete environment (Web + API + Infrastructure)'; Write-Host '  make up-web          Start Web application with infrastructure'; Write-Host '  make up-api          Start API services with infrastructure'; Write-Host '  make down            Stop all services'; Write-Host ''; Write-Host 'Development Commands:' -ForegroundColor Cyan; Write-Host '  make build           Build both Web and API projects'; Write-Host '  make test            Run all tests'; Write-Host '  make health          Check project and service health'; Write-Host '  make logs            View service logs'; Write-Host ''; Write-Host 'Setup Commands:' -ForegroundColor Cyan; Write-Host '  make setup           Generate Web and API projects'; Write-Host '  make clean           Clean all build outputs'"

up-fullstack: check-projects
	@powershell -NoProfile -Command "Write-Host 'Starting fullstack environment...' -ForegroundColor Green; $$env:COMPOSE_PROFILES='fullstack'; docker compose --env-file .env.$(ENV) -f docker-compose.fullstack.yml up -d --build; Write-Host 'Environment started! Web: http://localhost:$${WEB_HTTP_PORT:-3000} API: http://localhost:$${API_HTTP_PORT:-5000}' -ForegroundColor Cyan"

up-web: check-projects
	@powershell -NoProfile -Command "Write-Host 'Starting Web application with infrastructure...' -ForegroundColor Blue; $$env:COMPOSE_PROFILES='web-infra'; docker compose --env-file .env.$(ENV) -f docker-compose.fullstack.yml up -d --build; Write-Host 'Web application started! Web: http://localhost:$${WEB_HTTP_PORT:-3000}' -ForegroundColor Cyan"

up-api: check-projects
	@powershell -NoProfile -Command "Write-Host 'Starting API services with infrastructure...' -ForegroundColor Magenta; $$env:COMPOSE_PROFILES='backend-infra'; docker compose --env-file .env.$(ENV) -f docker-compose.fullstack.yml up -d --build; Write-Host 'API services started! API: http://localhost:$${API_HTTP_PORT:-5000}' -ForegroundColor Cyan"

check-projects:
	@powershell -NoProfile -Command "if (-not (Test-Path '$(WEB_PROJECT)') -or -not (Test-Path '$(API_PROJECT)')) { Write-Host '📦 Projects not found. Running setup...' -ForegroundColor Yellow; $$setupResult = & $(MAKE) setup; if ($$LASTEXITCODE -ne 0) { exit 1 } }"

up-dev:
	@$(MAKE) up-fullstack ENV=dev

up-staging:
	@$(MAKE) up-fullstack ENV=staging

down:
	@powershell -NoProfile -Command "Write-Host 'Stopping services...' -ForegroundColor Red; docker compose -f docker-compose.fullstack.yml down"

build: check-projects
	@powershell -NoProfile -Command "Write-Host 'Building Web application...' -ForegroundColor Blue; cd $(WEB_PROJECT); dotnet build; Write-Host 'Building API services...' -ForegroundColor Magenta; cd ../$(API_PROJECT); dotnet build; Write-Host 'Build completed!' -ForegroundColor Green"

test: check-projects
	@powershell -NoProfile -Command "Write-Host 'Testing Web application...' -ForegroundColor Blue; cd $(WEB_PROJECT); dotnet test; Write-Host 'Testing API services...' -ForegroundColor Magenta; cd ../$(API_PROJECT); dotnet test; Write-Host 'Tests completed!' -ForegroundColor Green"

setup:
	@powershell -NoProfile -Command "Write-Host 'Generating projects...' -ForegroundColor Yellow; .\setup.ps1"

clean: check-projects
	@powershell -NoProfile -Command "Write-Host 'Cleaning build outputs...' -ForegroundColor Yellow; cd $(WEB_PROJECT); dotnet clean; cd ../$(API_PROJECT); dotnet clean; Write-Host 'Clean completed!' -ForegroundColor Green"

logs:
	@docker compose -f docker-compose.fullstack.yml logs -f

health:
	@powershell -NoProfile -Command "Write-Host 'Checking project health...' -ForegroundColor Yellow; if (Test-Path '$(WEB_PROJECT)') { Write-Host 'Web project: ✅' -ForegroundColor Green } else { Write-Host 'Web project: ❌' -ForegroundColor Red }; if (Test-Path '$(API_PROJECT)') { Write-Host 'API project: ✅' -ForegroundColor Green } else { Write-Host 'API project: ❌' -ForegroundColor Red }; Write-Host 'Checking services...' -ForegroundColor Yellow; docker compose -f docker-compose.fullstack.yml ps"

.PHONY: help up-fullstack up-web up-api up-dev up-staging down build test setup clean logs health check-projects
.DEFAULT_GOAL := help
