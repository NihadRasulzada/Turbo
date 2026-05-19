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
MIGRATIONS_PROJECT_CAR      := src/Modules/Car/Infrastructure/Turbo.Module.Car.Persistence/Turbo.Module.Car.Persistence.csproj

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

# ── Help ──────────────────────────────────────────────────────
.PHONY: help
help:
	@echo ""
	@echo "  build            Solution-u build et"
	@echo "  build-release    Release modda build et"
	@echo "  run              API-ı işə sal"
	@echo "  watch            API-ı hot reload ilə işə sal"
	@echo "  test             Testləri işə sal"
	@echo "  restore          NuGet paketlərini restore et"
	@echo "  clean            bin/obj qovluqlarını sil"
	@echo "  format           Kodu formatla"
	@echo "  format-check     Format yoxlaması (CI üçün)"
	@echo "  mig-add          Migration əlavə et   → make mig-add NAME=Init MODULE=Identity"
	@echo "  mig-apply        Migration tətbiq et  → make mig-apply MODULE=Car"
	@echo "  mig-remove       Son migration-ı sil  → make mig-remove MODULE=Identity"
	@echo "  list             Solution-dakı layihələri göstər"
	@echo ""
