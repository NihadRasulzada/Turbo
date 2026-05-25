#!/bin/bash
# =====================================================
# Turbo - Command DB Init Script
# Docker initdb zamanı işləyir, environment dəyərləri
# avtomatik mövcuddur ($POSTGRES_* və custom env-lər)
# =====================================================

set -e

# POSTGRES_USER — Docker tərəfindən initdb zamanı set edilir.
# psql-ə eksplisit -U və -d veririk:
#   -U: bəzi image versiyalarında PGUSER=postgres fallback edir
#   -d: psql username-i DB adı kimi istifadə edir; POSTGRES_USER != POSTGRES_DB
#       olduqda "database does not exist" xətası verir.

DB_USER="${POSTGRES_USER:-turbo}"
DB_NAME="${POSTGRES_DB:-turbo_command}"
REPLICATOR_PASS="${REPLICATOR_PASSWORD:-replicator_password}"

echo "🔧 Replicator rolu yaradılır (user: $DB_USER, db: $DB_NAME)..."

psql -v ON_ERROR_STOP=1 -U "$DB_USER" -d "$DB_NAME" <<-SQL
    DO \$\$
    BEGIN
      IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'replicator') THEN
        CREATE ROLE replicator
          WITH REPLICATION
               LOGIN
               PASSWORD '${REPLICATOR_PASS}';
        RAISE NOTICE 'replicator rolu yaradıldı.';
      ELSE
        RAISE NOTICE 'replicator rolu artıq mövcuddur, keçilir.';
      END IF;
    END
    \$\$;

    GRANT pg_read_all_data TO replicator;
SQL

echo "✅ Init tamamlandı."
echo "   Replicator user: replicator"
echo "   WAL level: compose command ilə logical olaraq qurulub"
