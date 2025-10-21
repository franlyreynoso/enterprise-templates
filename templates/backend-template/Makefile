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

else
# (unix section unchanged)
endif
