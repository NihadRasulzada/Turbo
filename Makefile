SLN := Turbo.slnx
API := src/Presentation/Turbo.API/Turbo.API.csproj

# ── Build ─────────────────────────────────────────────────────
.PHONY: build
build:
	dotnet build $(SLN)

.PHONY: build-release
build-release:
	dotnet build $(SLN) -c Release

# ── Run ───────────────────────────────────────────────────────
.PHONY: run
run:
	dotnet run --project $(API)

.PHONY: watch
watch:
	dotnet watch --project $(API)

# ── Test ──────────────────────────────────────────────────────
.PHONY: test
test:
	dotnet test $(SLN)

.PHONY: test-verbose
test-verbose:
	dotnet test $(SLN) --logger "console;verbosity=detailed"

# ── Restore ───────────────────────────────────────────────────
.PHONY: restore
restore:
	dotnet restore $(SLN)

# ── Clean ─────────────────────────────────────────────────────
.PHONY: clean
clean:
	dotnet clean $(SLN)
	find . -type d \( -name bin -o -name obj \) -not -path "./.git/*" | xargs rm -rf

# ── Format ────────────────────────────────────────────────────
.PHONY: format
format:
	dotnet format $(SLN)

.PHONY: format-check
format-check:
	dotnet format $(SLN) --verify-no-changes

# ── EF Core ───────────────────────────────────────────────────
MODULE ?= Identity
MIGRATIONS_PROJECT_IDENTITY := src/Modules/Identity/Infrastructure/Turbo.Module.Identity.Persistence/Turbo.Module.Identity.Persistence.csproj
MIGRATIONS_PROJECT_CAR      := src/Modules/Catalog/Infrastructure/Turbo.Module.Catalog.Persistence/Turbo.Module.Catalog.Persistence.csproj
MIGRATIONS_PROJECT_CATALOG  := src/Modules/Catalog/Infrastructure/Turbo.Module.Catalog.Persistence/Turbo.Module.Catalog.Persistence.csproj
MIGRATIONS_PROJECT_MEDIA    := src/Modules/Media/Infrastructure/Turbo.Module.Media.Persistence/Turbo.Module.Media.Persistence.csproj

.PHONY: mig-add
mig-add:
	dotnet ef migrations add $(NAME) --project $(MIGRATIONS_PROJECT_$(MODULE)) --startup-project $(API)

.PHONY: mig-apply
mig-apply:
	dotnet ef database update --project $(MIGRATIONS_PROJECT_$(MODULE)) --startup-project $(API)

.PHONY: mig-remove
mig-remove:
	dotnet ef migrations remove --project $(MIGRATIONS_PROJECT_$(MODULE)) --startup-project $(API)

# ── List projects ─────────────────────────────────────────────
.PHONY: list
list:
	dotnet sln $(SLN) list

# ── Docker (Development) ──────────────────────────────────────
.PHONY: dev-up
dev-up:
	docker compose -f infra/docker-compose.yml --env-file infra/env/.env up -d

.PHONY: dev-down
dev-down:
	docker compose -f infra/docker-compose.yml --env-file infra/env/.env down

.PHONY: dev-logs
dev-logs:
	docker compose -f infra/docker-compose.yml logs -f

.PHONY: dev-reset
dev-reset:
	docker compose -f infra/docker-compose.yml --env-file infra/env/.env down -v

.PHONY: dev-ps
dev-ps:
	docker compose -f infra/docker-compose.yml ps

# ── Docker (Production) ───────────────────────────────────────
.PHONY: prod-build
prod-build:
	docker compose -f infra/docker-compose.prod.yml --env-file infra/env/.env.prod build

.PHONY: prod-up
prod-up:
	docker compose -f infra/docker-compose.prod.yml --env-file infra/env/.env.prod up -d

.PHONY: prod-down
prod-down:
	docker compose -f infra/docker-compose.prod.yml --env-file infra/env/.env.prod down

.PHONY: prod-logs
prod-logs:
	docker compose -f infra/docker-compose.prod.yml logs -f

# ── Redis ─────────────────────────────────────────────────────
.PHONY: redis-cli
redis-cli:
	docker exec -it turbo-redis redis-cli -a $$(grep REDIS_PASSWORD infra/env/.env | cut -d= -f2)

.PHONY: redis-flush
redis-flush:
	docker exec -it turbo-redis redis-cli -a $$(grep REDIS_PASSWORD infra/env/.env | cut -d= -f2) FLUSHALL

# ── RabbitMQ ──────────────────────────────────────────────────
.PHONY: rabbit-ui
rabbit-ui:
	@MGMT_PORT=$$(grep RABBITMQ_MANAGEMENT_PORT infra/env/.env | cut -d= -f2); \
	MGMT_PORT=$${MGMT_PORT:-15672}; \
	echo "RabbitMQ UI: http://localhost:$$MGMT_PORT"; \
	xdg-open "http://localhost:$$MGMT_PORT" 2>/dev/null || open "http://localhost:$$MGMT_PORT" 2>/dev/null || true

# ── MinIO ─────────────────────────────────────────────────────
.PHONY: minio-ui
minio-ui:
	@CONSOLE_PORT=$$(grep MINIO_CONSOLE_PORT infra/env/.env | cut -d= -f2); \
	CONSOLE_PORT=$${CONSOLE_PORT:-9001}; \
	echo "MinIO Console: http://localhost:$$CONSOLE_PORT"; \
	xdg-open "http://localhost:$$CONSOLE_PORT" 2>/dev/null || open "http://localhost:$$CONSOLE_PORT" 2>/dev/null || true

# ── Setup ─────────────────────────────────────────────────────
.PHONY: setup
setup:
	@if [ ! -f infra/env/.env ]; then \
		cp infra/env/.env.example infra/env/.env; \
		echo "✅ infra/env/.env yaradıldı. Dəyərləri doldurun."; \
	else \
		echo "⚠️  infra/env/.env artıq mövcuddur."; \
	fi
	@if [ ! -f infra/env/.env.prod ]; then \
		cp infra/env/.env.prod.example infra/env/.env.prod; \
		echo "✅ infra/env/.env.prod yaradıldı. Dəyərləri doldurun."; \
	else \
		echo "⚠️  infra/env/.env.prod artıq mövcuddur."; \
	fi

# ── Help ──────────────────────────────────────────────────────
.PHONY: help
help:
	@echo ""
	@echo "  ── .NET ──────────────────────────────────────────────────"
	@echo "  build            Solution-u build et"
	@echo "  build-release    Release modda build et"
	@echo "  run              API-ı işə sal"
	@echo "  watch            API-ı hot reload ilə işə sal"
	@echo "  test             Testləri işə sal"
	@echo "  test-verbose     Testləri ətraflı çıxışla işə sal"
	@echo "  restore          NuGet paketlərini restore et"
	@echo "  clean            bin/obj qovluqlarını sil"
	@echo "  format           Kodu formatla"
	@echo "  format-check     Format yoxlaması (CI üçün)"
	@echo "  mig-add          Migration əlavə et   → make mig-add NAME=Init MODULE=Identity"
	@echo "  mig-apply        Migration tətbiq et  → make mig-apply MODULE=Car"
	@echo "  mig-remove       Son migration-ı sil  → make mig-remove MODULE=Identity"
	@echo "  list             Solution-dakı layihələri göstər"
	@echo ""
	@echo "  ── Docker (Development) ──────────────────────────────────"
	@echo "  dev-up           Bütün servisləri qaldır (DB, Redis, RabbitMQ)"
	@echo "  dev-down         Servisləri dayandır"
	@echo "  dev-logs         Log-ları izlə"
	@echo "  dev-reset        Servisləri volume-larla birlikdə sıfırla"
	@echo "  dev-ps           Çalışan containerləri göstər"
	@echo ""
	@echo "  ── Docker (Production) ───────────────────────────────────"
	@echo "  prod-build       Production image-i build et"
	@echo "  prod-up          Bütün sistemi production-da qaldır"
	@echo "  prod-down        Production-u dayandır"
	@echo "  prod-logs        Production log-larını izlə"
	@echo ""
	@echo "  ── Redis ─────────────────────────────────────────────────"
	@echo "  redis-cli        Redis CLI-a qoşul"
	@echo "  redis-flush      Redis cache-i təmizlə"
	@echo ""
	@echo "  ── RabbitMQ ──────────────────────────────────────────────"
	@echo "  rabbit-ui        Management UI-ı brauzerdə aç (http://localhost:15672)"
	@echo ""
	@echo "  ── MinIO ─────────────────────────────────────────────────"
	@echo "  minio-ui         Console UI-ı brauzerdə aç  (http://localhost:9001)"
	@echo ""
	@echo "  ── Setup ─────────────────────────────────────────────────"
	@echo "  setup            .env fayllarını nümunədən yarat"
	@echo ""