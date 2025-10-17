#!/bin/bash
set -e

HOST=${MSSQL_HOST:-db}
USER=sa
PWD=${SA_PASSWORD}
MAX_TRIES=30
SLEEP_SECONDS=5

i=0
until /opt/mssql-tools/bin/sqlcmd -S ${HOST},1433 -U ${USER} -P "${PWD}" -Q "SELECT 1" >/dev/null 2>&1; do
  i=$((i+1))
  if [ $i -ge $MAX_TRIES ]; then
    echo "SQL Server did not start in time"
    exit 1
  fi
  echo "Waiting for SQL Server to be ready... (attempt $i)"
  sleep ${SLEEP_SECONDS}
done

echo "Running FUNewsManagement.sql"
/opt/mssql-tools/bin/sqlcmd -S ${HOST},1433 -U ${USER} -P "${PWD}" -i /scripts/FUNewsManagement.sql

echo "Database initialization complete." 
