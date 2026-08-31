#!/bin/zsh
# Script para configurar los user secrets necesarios para SourceGuild.API
# Ejecuta este script desde la raíz del proyecto

PROJECT_PATH="source-guild-backend/SourceGuild.API/SourceGuild.API.csproj"

# Cadena de conexión plantilla
USER_SECRET_CONN="Server=localhost;Database=SourceGuildDB;TrustServerCertificate=True"
USER_SECRET_USER="sa"
USER_SECRET_PASS="AmeatoCondess20"

dotnet user-secrets set "ConnectionStrings:SourceGuildDbConnection_Template" "$USER_SECRET_CONN" --project $PROJECT_PATH

dotnet user-secrets set "DbCredentials:UserId" "$USER_SECRET_USER" --project $PROJECT_PATH

dotnet user-secrets set "DbCredentials:Password" "$USER_SECRET_PASS" --project $PROJECT_PATH

dotnet user-secrets set "SeedAdminCredentials:Email" "admin@sourceguild.com" --project $PROJECT_PATH
dotnet user-secrets set "SeedAdminCredentials:Password" "IAmAdmin@123" --project $PROJECT_PATH
dotnet user-secrets set "JwtSettings:SecretKey" "ThisIsASecretKey4SourceGuildProject4UserAuthenticationAndAuthorization" --project $PROJECT_PATH
echo "User secrets configurados para SourceGuild.API"
