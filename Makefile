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
MODULE ?= Catalog
# MODULE-u həmişə böyük hərfə çeviririk (Catalog → CATALOG, media → MEDIA)
MODULE_UPPER := $(shell echo $(MODULE) | tr '[:lower:]' '[:upper:]')

# Persistence layihə yolları
MIGRATIONS_PROJECT_IDENTITY := src/Modules/Identity/Infrastructure/Turbo.Module.Identity.Persistence/Turbo.Module.Identity.Persistence.csproj
MIGRATIONS_PROJECT_CATALOG  := src/Modules/Catalog/Infrastructure/Turbo.Module.Catalog.Persistence/Turbo.Module.Catalog.Persistence.csproj
MIGRATIONS_PROJECT_MEDIA    := src/Modules/Media/Infrastructure/Turbo.Module.Media.Persistence/Turbo.Module.Media.Persistence.csproj

# Fully qualified context adları (eyni adlı context-lərin qarışmaması üçün)
COMMAND_CONTEXT_IDENTITY := Turbo.Module.Identity.Persistence.Contexts.CommandDbContext
QUERY_CONTEXT_IDENTITY   := Turbo.Module.Identity.Persistence.Contexts.QueryDbContext
COMMAND_CONTEXT_CATALOG  := Turbo.Module.Catalog.Persistence.Contexts.CommandDbContext
QUERY_CONTEXT_CATALOG    := Turbo.Module.Catalog.Persistence.Contexts.QueryDbContext
COMMAND_CONTEXT_MEDIA    := Turbo.Module.Media.Persistence.Contexts.CommandDbContext
QUERY_CONTEXT_MEDIA      := Turbo.Module.Media.Persistence.Contexts.QueryDbContext

MIGRATIONS_PROJECT := $(MIGRATIONS_PROJECT_$(MODULE_UPPER))
COMMAND_CONTEXT    := $(COMMAND_CONTEXT_$(MODULE_UPPER))
QUERY_CONTEXT      := $(QUERY_CONTEXT_$(MODULE_UPPER))

# Daxili yardımçılar — MODULE və NAME yoxlanması
define check-module
	@if [ -z "$(MIGRATIONS_PROJECT)" ]; then \
		echo "❌ MODULE='$(MODULE)' tanınmır. İstifadə et: Catalog, Media, Identity"; exit 1; \
	fi
endef

define check-name
	@if [ -z "$(NAME)" ]; then \
		echo "❌ NAME parametri lazımdır. Nümunə: make mig-add NAME=Init MODULE=Catalog"; exit 1; \
	fi
endef

.PHONY: mig-add
mig-add:
	$(call check-module)
	$(call check-name)
	@echo "📦 [$(MODULE)] CommandDbContext üçün migration yaradılır: $(NAME)"
	dotnet ef migrations add $(NAME) \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(API) \
		--context $(COMMAND_CONTEXT) \
		--output-dir Migrations/Command
	@echo "📦 [$(MODULE)] QueryDbContext üçün migration yaradılır: $(NAME)"
	dotnet ef migrations add $(NAME) \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(API) \
		--context $(QUERY_CONTEXT) \
		--output-dir Migrations/Query
	@echo "✅ Migration faylları yaradıldı."

.PHONY: mig-apply
mig-apply:
	$(call check-module)
	@echo "🚀 [$(MODULE)] CommandDbContext (write DB) tətbiq edilir..."
	dotnet ef database update \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(API) \
		--context $(COMMAND_CONTEXT)
	@echo "🚀 [$(MODULE)] QueryDbContext (read DB) tətbiq edilir..."
	dotnet ef database update \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(API) \
		--context $(QUERY_CONTEXT)
	@echo "✅ Hər iki DB yeniləndi."

.PHONY: mig-remove
mig-remove:
	$(call check-module)
	@echo "🗑️  [$(MODULE)] CommandDbContext son migration silinir..."
	dotnet ef migrations remove \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(API) \
		--context $(COMMAND_CONTEXT)
	@echo "🗑️  [$(MODULE)] QueryDbContext son migration silinir..."
	dotnet ef migrations remove \
		--project $(MIGRATIONS_PROJECT) \
		--startup-project $(API) \
		--context $(QUERY_CONTEXT)
	@echo "✅ Son migrationlar silindi."

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
	@echo "  mig-add          Migration yarat   → make mig-add NAME=Init MODULE=Catalog"
	@echo "  mig-apply        Hər 2 DB-yə tətbiq et → make mig-apply MODULE=Catalog"
	@echo "  mig-remove       Son migration-ı sil  → make mig-remove MODULE=Catalog"
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