#!/bin/bash

# Ensure a SQL Server container is running (support renamed container)
CONTAINER_NAME=""
if [ "$(docker ps -q -f name=sourceguild-sql)" != "" ]; then
  CONTAINER_NAME=sourceguild-sql
elif [ "$(docker ps -q -f name=sqlserver)" != "" ]; then
  CONTAINER_NAME=sqlserver
fi

if [ -z "$CONTAINER_NAME" ]; then
  echo "No existing SQL Server container found. Starting via docker-compose..."
  docker-compose up -d
  # try to pick up the container name we just started
  if [ "$(docker ps -q -f name=sourceguild-sql)" != "" ]; then
    CONTAINER_NAME=sourceguild-sql
  fi
fi

if [ -n "$CONTAINER_NAME" ]; then
  # If the container exists but is stopped, start it
  if [ "$(docker ps -q -f name=$CONTAINER_NAME)" = "" ]; then
    echo "Starting $CONTAINER_NAME container..."
    docker start $CONTAINER_NAME || true
  else
    echo "$CONTAINER_NAME container is already running."
  fi
fi

# Check for dotnet-ef and install if missing
if ! dotnet ef --version >/dev/null 2>&1; then
  echo "dotnet-ef not found globally. Attempting to install dotnet-ef tool (user scope)..."
  dotnet tool install --global dotnet-ef || true
  export PATH="$PATH:$HOME/.dotnet/tools"
fi

# Prompt for migration name
read -p "Enter migration name (or press Enter to use 'InitialCreate'): " MIGRATION_NAME
if [ -z "$MIGRATION_NAME" ]; then
  MIGRATION_NAME="InitialCreate"
fi

# Run dotnet ef migrations add
if [ -z "$MIGRATION_NAME" ]; then
  echo "Migration name cannot be empty. Exiting."
  exit 1
fi

echo "Adding migration '$MIGRATION_NAME' to SourceGuild.Infrastructure (startup: SourceGuild.API)"
dotnet ef migrations add "$MIGRATION_NAME" --project SourceGuild.Infrastructure --startup-project SourceGuild.API || true

# Run dotnet ef database update
echo "Applying migrations to database (update)..."
dotnet ef database update --project SourceGuild.Infrastructure --startup-project SourceGuild.API || true
