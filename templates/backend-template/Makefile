# --- Windows (PowerShell) vs Unix (sh) detection ---
ifeq ($(OS),Windows_NT)
SHELL := pwsh.exe
.SHELLFLAGS := -NoProfile -Command
COMPOSE_FILE := docker-compose.envs.yml

check-compose:
	@if (-not (Get-Command docker -ErrorAction SilentlyContinue)) { Write-Error '❌ Docker CLI not found.'; exit 1 }; if (-not ((docker compose version) 2>$$null)) { Write-Error '❌ `docker compose` not available. Enable Compose V2 in Docker Desktop or install the plugin.'; exit 1 }

up-dev: check-compose
	$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60


down-dev: check-compose
	$$env:COMPOSE_PROFILES='dev'; docker compose --env-file .env.dev -f $(COMPOSE_FILE) down -v --remove-orphans

up-staging: check-compose
	$$env:COMPOSE_PROFILES='staging'; docker compose --env-file .env.staging -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

down-staging: check-compose
	$$env:COMPOSE_PROFILES='staging'; docker compose --env-file .env.staging -f $(COMPOSE_FILE) down -v --remove-orphans

up-prod: check-compose
	$$env:COMPOSE_PROFILES='prod'; docker compose --env-file .env.prod -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

down-prod: check-compose
	$$env:COMPOSE_PROFILES='prod'; docker compose --env-file .env.prod -f $(COMPOSE_FILE) down -v --remove-orphans

logs: check-compose
	docker compose -f $(COMPOSE_FILE) logs -f

ps: check-compose
	docker compose -f $(COMPOSE_FILE) ps

clean: check-compose
	$$env:COMPOSE_PROFILES='dev,staging,prod'; docker compose -f $(COMPOSE_FILE) down -v --remove-orphans

help:
	@Write-Host '🚀 Enterprise Backend Template - Make Commands' -ForegroundColor Green; Write-Host ''; Write-Host 'Environment Commands:' -ForegroundColor Cyan; Write-Host '  make up-dev       Start development environment'; Write-Host '  make down-dev     Stop development environment'; Write-Host '  make up-staging   Start staging environment'; Write-Host '  make down-staging Stop staging environment'; Write-Host '  make up-prod      Start production environment'; Write-Host '  make down-prod    Stop production environment'; Write-Host ''; Write-Host 'Monitoring Commands:' -ForegroundColor Cyan; Write-Host '  make logs         View all service logs'; Write-Host '  make ps           Show running services'; Write-Host ''; Write-Host 'Cleanup Commands:' -ForegroundColor Cyan; Write-Host '  make clean        Stop all environments and remove volumes'

else
# Unix (Linux/macOS) shell commands
SHELL := /bin/bash
COMPOSE_FILE := docker-compose.envs.yml

check-compose:
	@command -v docker >/dev/null 2>&1 || { echo "❌ Docker CLI not found."; exit 1; }; docker compose version >/dev/null 2>&1 || { echo "❌ docker compose not available."; exit 1; }

up-dev: check-compose
	COMPOSE_PROFILES=dev docker compose --env-file .env.dev -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

down-dev: check-compose
	COMPOSE_PROFILES=dev docker compose --env-file .env.dev -f $(COMPOSE_FILE) down -v --remove-orphans

up-staging: check-compose
	COMPOSE_PROFILES=staging docker compose --env-file .env.staging -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

down-staging: check-compose
	COMPOSE_PROFILES=staging docker compose --env-file .env.staging -f $(COMPOSE_FILE) down -v --remove-orphans

up-prod: check-compose
	COMPOSE_PROFILES=prod docker compose --env-file .env.prod -f $(COMPOSE_FILE) up -d --wait --wait-timeout 60

down-prod: check-compose
	COMPOSE_PROFILES=prod docker compose --env-file .env.prod -f $(COMPOSE_FILE) down -v --remove-orphans

logs: check-compose
	docker compose -f $(COMPOSE_FILE) logs -f

ps: check-compose
	docker compose -f $(COMPOSE_FILE) ps

clean: check-compose
	COMPOSE_PROFILES=dev,staging,prod docker compose -f $(COMPOSE_FILE) down -v --remove-orphans

help:
	@echo "🚀 Enterprise Backend Template - Make Commands"; echo ""; echo "Environment Commands:"; echo "  make up-dev       Start development environment"; echo "  make down-dev     Stop development environment"; echo "  make up-staging   Start staging environment"; echo "  make down-staging Stop staging environment"; echo "  make up-prod      Start production environment"; echo "  make down-prod    Stop production environment"; echo ""; echo "Monitoring Commands:"; echo "  make logs         View all service logs"; echo "  make ps           Show running services"; echo ""; echo "Cleanup Commands:"; echo "  make clean        Stop all environments and remove volumes"

endif

.PHONY: check-compose up-dev down-dev up-staging down-staging up-prod down-prod logs ps clean help
