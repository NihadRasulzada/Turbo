# =====================================================
# Turbo API - Multi-stage Dockerfile
# =====================================================

# ── Build Stage ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /repo

COPY . .

RUN dotnet restore src/Presentation/Turbo.API/Turbo.API.csproj

RUN dotnet publish src/Presentation/Turbo.API/Turbo.API.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# ── Runtime Stage ────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Non-root user üçün təhlükəsizlik
RUN addgroup -S turbo && adduser -S turbo -G turbo
USER turbo

COPY --from=build --chown=turbo:turbo /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Turbo.API.dll"]