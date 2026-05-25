#!/bin/bash
# =====================================================
# Turbo - Logical Replication Wiring Script
# Runs AFTER both command-db and query-db are healthy.
# Called by the replication-setup service in compose.
#
# Bu script YALNIZ command-db-də publication yaradır.
# Subscription app startup-da (Program.cs) migration-
# lardan SONRA yaradılır — timing problemini aradan qaldırır.
# =====================================================

set -e

COMMAND_HOST="${COMMAND_DB_HOST:-command-db}"
COMMAND_PORT="${COMMAND_DB_PORT:-5432}"
DB_USER="${POSTGRES_USER:-turbo}"
DB_PASS="${POSTGRES_PASSWORD:-turbo_password}"
COMMAND_DB="${COMMAND_DB_NAME:-turbo_command}"

export PGPASSWORD="$DB_PASS"

echo "⏳ Waiting for command-db..."
until pg_isready -h "$COMMAND_HOST" -p "$COMMAND_PORT" -U "$DB_USER"; do
    sleep 2
done

echo "✅ command-db is ready."

# ── Create Publication on Command DB ──────────────────
echo "📢 Creating publication on command-db..."
psql -h "$COMMAND_HOST" -p "$COMMAND_PORT" -U "$DB_USER" -d "$COMMAND_DB" \
     -v ON_ERROR_STOP=1 <<SQL
DO \$\$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_publication WHERE pubname = 'turbo_publication'
  ) THEN
    CREATE PUBLICATION turbo_publication FOR ALL TABLES;
    RAISE NOTICE 'Publication created.';
  ELSE
    RAISE NOTICE 'Publication already exists, skipping.';
  END IF;
END
\$\$;
SQL

echo "✅ Publication hazırdır."
echo "   Publisher: $COMMAND_HOST/$COMMAND_DB"
echo ""
echo "ℹ️  Subscription app startup-da (Program.cs → SetupReplicationAsync)"
echo "   migration-lardan SONRA yaradılacaq."
