#!/usr/bin/env bash
# scripts/setup-secrets.sh - Configura User Secrets locales leyendo variables de .env

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
ENV_FILE="$REPO_ROOT/.env"
PROJECT_PATH="$REPO_ROOT/src/SourceGuild.API"

if [[ ! -f "$ENV_FILE" ]]; then
    echo "Error: No se encontró el archivo .env en $REPO_ROOT"
    echo "Crea un archivo .env copiando .env.example"
    exit 1
fi

# Cargar variables de .env
export $(grep -v '^#' "$ENV_FILE" | xargs)

echo "Inicializando y configurando User Secrets para SourceGuild.API..."

dotnet user-secrets init --project "$PROJECT_PATH"

dotnet user-secrets set "ConnectionStrings:SourceGuildDbConnection_Template" "Server=localhost,${MSSQL_PORT:-1433};Database=SourceGuildDB;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=True;MultipleActiveResultSets=true" --project "$PROJECT_PATH"
dotnet user-secrets set "DbCredentials:UserId" "sa" --project "$PROJECT_PATH"
dotnet user-secrets set "DbCredentials:Password" "${MSSQL_SA_PASSWORD}" --project "$PROJECT_PATH"
dotnet user-secrets set "SeedAdminCredentials:Email" "${ADMIN_EMAIL:-admin@sourceguild.com}" --project "$PROJECT_PATH"
dotnet user-secrets set "SeedAdminCredentials:Password" "${ADMIN_PASSWORD:-AdminPass123!}" --project "$PROJECT_PATH"
dotnet user-secrets set "JwtSettings:SecretKey" "${JWT_SECRET:-SuperSecretKeyForSourceGuildProject2026AuthToken}" --project "$PROJECT_PATH"

echo "User Secrets configurados exitosamente."
EOF

