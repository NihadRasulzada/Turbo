#!/bin/bash
# =====================================================
# Turbo - Query DB Init Script
# Docker initdb zamanı işləyir.
# turbo_reader — yalnız SELECT icazəsi olan runtime user.
# App bu user ilə qoşulur; admin user (turbo) yalnız
# EF Core migrations üçün istifadə olunur.
# =====================================================

set -e

READER_PASS="${QUERY_READER_PASSWORD:-turbo_reader_password}"
DB_NAME="${QUERY_DB_NAME:-turbo_query}"
ADMIN_USER="${QUERY_DB_USER:-turbo}"

echo "🔧 turbo_reader rolu yaradılır..."

psql -v ON_ERROR_STOP=1 -U "$ADMIN_USER" -d "$DB_NAME" <<-SQL
    DO \$\$
    BEGIN
      IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'turbo_reader') THEN
        CREATE ROLE turbo_reader
          WITH LOGIN
               PASSWORD '${READER_PASS}';
        RAISE NOTICE 'turbo_reader rolu yaradıldı.';
      ELSE
        ALTER ROLE turbo_reader WITH PASSWORD '${READER_PASS}';
        RAISE NOTICE 'turbo_reader rolu artıq mövcuddur, şifrə yeniləndi.';
      END IF;
    END
    \$\$;

    -- Qoşulma icazəsi
    GRANT CONNECT ON DATABASE "${DB_NAME}" TO turbo_reader;

    -- Schema istifadə icazəsi
    GRANT USAGE ON SCHEMA public TO turbo_reader;

    -- Mövcud cədvəllərə SELECT icazəsi
    GRANT SELECT ON ALL TABLES IN SCHEMA public TO turbo_reader;

    -- Gələcəkdə yaradılacaq cədvəllərə də avtomatik SELECT icazəsi
    ALTER DEFAULT PRIVILEGES IN SCHEMA public
        GRANT SELECT ON TABLES TO turbo_reader;
SQL

echo "✅ turbo_reader hazırdır."
echo "   Role: turbo_reader (SELECT-only)"
echo "   Database: ${DB_NAME}"
