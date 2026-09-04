#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
ENV_FILE="$REPO_ROOT/.env"
PROJECT_PATH="$REPO_ROOT/src/SourceGuild.API"

if [[ ! -f "$ENV_FILE" ]]; then
    echo "Error: No se encontró el archivo .env en $REPO_ROOT"
    exit 1
fi

# Cargar variables de entorno ignorando comentarios
set -a
source "$ENV_FILE"
set +a

echo "Configurando User Secrets para SourceGuild.API..."

dotnet user-secrets init --project "$PROJECT_PATH" 2>/dev/null || true

# Configuración de ConnectionString
dotnet user-secrets set "ConnectionStrings:SourceGuildDbConnection_Template" "Server=localhost,${MSSQL_PORT};Database=SourceGuildDB;User Id=sa;Password=${MSSQL_SA_PASSWORD};TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true" --project "$PROJECT_PATH"
dotnet user-secrets set "DbCredentials:UserId" "sa" --project "$PROJECT_PATH"
dotnet user-secrets set "DbCredentials:Password" "${MSSQL_SA_PASSWORD}" --project "$PROJECT_PATH"
dotnet user-secrets set "SeedAdminCredentials:Email" "${ADMIN_EMAIL}" --project "$PROJECT_PATH"
dotnet user-secrets set "SeedAdminCredentials:Password" "${ADMIN_PASSWORD}" --project "$PROJECT_PATH"
dotnet user-secrets set "JwtSettings:SecretKey" "${JWT_SECRET}" --project "$PROJECT_PATH"

echo "User Secrets configurados exitosamente."