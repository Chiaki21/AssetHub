# PowerShell setup script for AssetHub
# 1. Start Docker containers
docker-compose up -d

# 2. Wait for SQL Server to be ready
echo "Waiting for SQL Server to start..."
Start-Sleep -Seconds 10

# 3. Drop the database if it exists (ignore errors)
docker run --rm --network host mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "YourStrongPassword123!" -Q "IF DB_ID('AssetHubDB') IS NOT NULL DROP DATABASE AssetHubDB;"

# 4. Apply EF Core migrations
dotnet ef database update

echo "Setup complete. You can now run the application."
