#!/bin/bash
# =====================================================
# Turbo - Command DB Init Script
# Docker initdb zamanı işləyir, environment dəyərləri
# avtomatik mövcuddur ($POSTGRES_* və custom env-lər)
# =====================================================

set -e

# Docker-in özü POSTGRES_USER ilə qoşulur, psql əmrləri
# həmin user adından işləyir — ayrıca -U lazım deyil.

REPLICATOR_PASS="${REPLICATOR_PASSWORD:-replicator_password}"

echo "🔧 Replicator rolu yaradılır..."

psql -v ON_ERROR_STOP=1 <<-SQL
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
echo "   WAL level logical olması üçün compose command: baxın docker-compose.yml"
