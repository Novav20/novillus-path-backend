#!/bin/bash

# Start sqlserver container if not running
if [ "$(docker ps -q -f name=sqlserver)" = "" ]; then
  echo "Starting sqlserver container..."
  docker start sqlserver
else
  echo "sqlserver container is already running."
fi

# Prompt for migration name
read -p "Enter migration name: " MIGRATION_NAME

# Run dotnet ef migrations add
if [ -z "$MIGRATION_NAME" ]; then
  echo "Migration name cannot be empty. Exiting."
  exit 1
fi

dotnet ef migrations add "$MIGRATION_NAME" -p NovillusPath.Infrastructure -s NovillusPath.API

# Run dotnet ef database update
dotnet ef database update -p NovillusPath.Infrastructure -s NovillusPath.API
