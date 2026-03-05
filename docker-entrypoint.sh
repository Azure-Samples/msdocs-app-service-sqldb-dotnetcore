#!/usr/bin/env sh
set -eu

echo "Applying EF migrations bundle..."
tries=0
max_tries=30
if [ -n "${AZURE_SQL_CONNECTIONSTRING:-}" ]; then
  migrate_cmd="./migrationsbundle --connection \"$AZURE_SQL_CONNECTIONSTRING\" -- --environment Production"
else
  migrate_cmd="./migrationsbundle -- --environment Production"
fi

until sh -c "$migrate_cmd"; do
  tries=$((tries + 1))
  if [ "$tries" -ge "$max_tries" ]; then
    echo "Migrations failed after $max_tries attempts."
    exit 1
  fi
  echo "Migration attempt $tries failed; retrying in 5s..."
  sleep 5
done

echo "Starting web app..."
exec dotnet DotNetCoreSqlDb.dll
