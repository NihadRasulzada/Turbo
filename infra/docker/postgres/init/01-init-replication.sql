
-- =====================================================
-- Turbo - PostgreSQL Logical Replication Setup
-- Command DB (Publisher) → Query DB (Subscriber)
-- =====================================================

-- This script runs on the COMMAND DB (publisher).
-- It creates the publication for all tables.
-- The subscriber setup is handled separately after both DBs are ready.

-- Create the application databases
SELECT 'CREATE DATABASE turbo_command'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'turbo_command')\gexec

SELECT 'CREATE DATABASE turbo_query'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'turbo_query')\gexec

-- Create replication user
DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'replicator') THEN
    CREATE ROLE replicator WITH REPLICATION LOGIN PASSWORD 'replicator_password';
  END IF;
END
$$;

GRANT pg_read_all_data TO replicator;