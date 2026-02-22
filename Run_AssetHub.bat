@echo off
echo Starting AssetHub Database...
docker-compose up -d
echo Database is ready.
echo.
echo Launching AssetHub Application...
:: This just looks in the current folder, no "bin/release" needed
start "" "AssetHub.exe"
exit