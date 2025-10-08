#!/bin/sh
echo "[Entrypoint] Starting container..."
echo "[Entrypoint] PORT=${PORT:-(not set)}"
echo "[Entrypoint] ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-(not set)}"
echo "[Entrypoint] Listing /app contents:" 
ls -la /app || true
echo "[Entrypoint] Executing dotnet with urls=http://0.0.0.0:${PORT:-80}"
exec dotnet CsvToVcfConverter.dll --urls "http://0.0.0.0:${PORT:-80}"
