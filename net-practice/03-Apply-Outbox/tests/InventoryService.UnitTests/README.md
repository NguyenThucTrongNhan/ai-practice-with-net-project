# InventoryService.UnitTests

Run unit tests:

dotnet test ./tests/InventoryService.UnitTests/InventoryService.UnitTests.csproj

Run tests with coverage (coverlet collector):
dotnet test ./tests/InventoryService.UnitTests/InventoryService.UnitTests.csproj --collect:"XPlat Code Coverage"

Generate html report with ReportGenerator (optional):
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
