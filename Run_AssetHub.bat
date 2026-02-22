@echo off
echo Starting AssetHub Database...
docker-compose up -d
echo Database is ready.
echo.
echo Launching AssetHub Application...
start "" "bin\Release\net8.0-windows\AssetHub.exe"
exit