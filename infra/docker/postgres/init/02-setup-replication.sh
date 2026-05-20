#!/bin/bash
# =====================================================
# Turbo - Logical Replication Wiring Script
# Runs AFTER both command-db and query-db are healthy
# Called by the replication-setup service in compose
# =====================================================

set -e

COMMAND_HOST="${COMMAND_DB_HOST:-command-db}"
QUERY_HOST="${QUERY_DB_HOST:-query-db}"
COMMAND_PORT="${COMMAND_DB_PORT:-5432}"
QUERY_PORT="${QUERY_DB_PORT:-5432}"
DB_USER="${POSTGRES_USER:-turbo}"
DB_PASS="${POSTGRES_PASSWORD:-turbo_password}"
COMMAND_DB="${COMMAND_DB_NAME:-turbo_command}"
QUERY_DB="${QUERY_DB_NAME:-turbo_query}"
REPLICATOR_PASS="${REPLICATOR_PASSWORD:-replicator_password}"

export PGPASSWORD="$DB_PASS"

echo "⏳ Waiting for command-db..."
until pg_isready -h "$COMMAND_HOST" -p "$COMMAND_PORT" -U "$DB_USER"; do
        sleep 2
done

echo "⏳ Waiting for query-db..."
until pg_isready -h "$QUERY_HOST" -p "$QUERY_PORT" -U "$DB_USER"; do
        sleep 2
done

echo "✅ Both databases are ready."

# ── Create Publication on Command DB ──────────────────
echo "📢 Creating publication on command-db..."
psql -h "$COMMAND_HOST" -p "$COMMAND_PORT" -U "$DB_USER" -d "$COMMAND_DB" <<SQL
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

# ── Create Subscription on Query DB ───────────────────
echo "🔗 Creating subscription on query-db..."
psql -h "$QUERY_HOST" -p "$QUERY_PORT" -U "$DB_USER" -d "$QUERY_DB" <<SQL
DO \$\$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_subscription WHERE subname = 'turbo_subscription'
  ) THEN
    CREATE SUBSCRIPTION turbo_subscription
      CONNECTION 'host=${COMMAND_HOST} port=${COMMAND_PORT} dbname=${COMMAND_DB} user=replicator password=${REPLICATOR_PASS}'
      PUBLICATION turbo_publication;
    RAISE NOTICE 'Subscription created.';
  ELSE
    RAISE NOTICE 'Subscription already exists, skipping.';
  END IF;
END
\$\$;
SQL

echo "✅ Logical replication wired successfully."
echo "   Publisher : $COMMAND_HOST/$COMMAND_DB"
echo "   Subscriber: $QUERY_HOST/$QUERY_DB"