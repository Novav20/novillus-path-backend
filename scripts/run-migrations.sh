#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

cd "$REPO_ROOT"

CONTAINER_NAME="sourceguild-sqlserver"

echo "Verificando contenedor SQL Server ($CONTAINER_NAME)..."

# 1. Asegurar que el contenedor esté corriendo en Docker
if ! docker ps -q -f name="^${CONTAINER_NAME}$" | grep -q .; then
    echo "Contenedor no detectado activo. Levantando servicios vía docker compose..."
    docker compose up -d
fi

# 2. Verificar herramienta dotnet-ef
if ! dotnet ef --version >/dev/null 2>&1; then
    echo "Instalando herramienta global dotnet-ef..."
    dotnet tool install --global dotnet-ef || true
    export PATH="$PATH:$HOME/.dotnet/tools"
fi

# 3. Preguntar acción
echo ""
echo "Seleccione una opción:"
echo "1) Aplicar migraciones existentes (database update)"
echo "2) Crear nueva migración y aplicar (migrations add + update)"
read -p "Opción [1/2, default: 1]: " OPTION
OPTION=${OPTION:-1}

if [[ "$OPTION" == "2" ]]; then
    read -p "Ingrese el nombre de la nueva migración: " MIGRATION_NAME
    if [[ -z "$MIGRATION_NAME" ]]; then
        echo "Error: El nombre de la migración no puede estar vacío."
        exit 1
    fi

    echo "Generando migración '$MIGRATION_NAME'..."
    dotnet ef migrations add "$MIGRATION_NAME" \
        --project src/SourceGuild.Infrastructure \
        --startup-project src/SourceGuild.API
fi

echo "Aplicando migraciones a la base de datos..."
dotnet ef database update \
    --project src/SourceGuild.Infrastructure \
    --startup-project src/SourceGuild.API

echo "Operación de base de datos completada exitosamente."