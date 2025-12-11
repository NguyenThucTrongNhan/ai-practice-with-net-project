# ================================================
# Law Firm Modular Monolith Project Generator
# Backend: .NET 9
# Generates: Folder structure + .sln + csprojs
# ================================================

$solutionName = "LawFirmSolution"
$root = "backend"
$modules = @(
    "Auth",
    "Clients",
    "CaseManagement",
    "Billing",
    "Documents",
    "CRM",
    "Workflow",
    "Reports",
    "HR"
)

Write-Host "Creating backend root folders..." -ForegroundColor Cyan

# Root folders
New-Item -ItemType Directory -Force -Path $root
New-Item -ItemType Directory -Force -Path "$root/src"
New-Item -ItemType Directory -Force -Path "$root/src/ApiHost"
New-Item -ItemType Directory -Force -Path "$root/src/BuildingBlocks"
New-Item -ItemType Directory -Force -Path "$root/src/Modules"
New-Item -ItemType Directory -Force -Path "$root/tests"

# ===========================================
# Create base files in ApiHost
# ===========================================
New-Item "$root/src/ApiHost/Program.cs" -ItemType File -Value @"
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/", () => "API Host Running...");
app.Run();
"@

New-Item "$root/src/ApiHost/Startup.cs" -ItemType File
New-Item "$root/src/ApiHost/appsettings.json" -ItemType File -Value "{}"

# ===========================================
# Create BuildingBlocks structure
# ===========================================
$buildingBlocks = @(
    "Domain",
    "Application",
    "Infrastructure",
    "Shared"
)

foreach ($bb in $buildingBlocks) {
    New-Item -ItemType Directory -Force -Path "$root/src/BuildingBlocks/$bb"
}

# Common base files
New-Item "$root/src/BuildingBlocks/Domain/Entity.cs" -ItemType File
New-Item "$root/src/BuildingBlocks/Domain/ValueObject.cs" -ItemType File
New-Item "$root/src/BuildingBlocks/Domain/DomainEvent.cs" -ItemType File

New-Item "$root/src/BuildingBlocks/Application/ICommand.cs" -ItemType File
New-Item "$root/src/BuildingBlocks/Application/IQuery.cs" -ItemType File
New-Item "$root/src/BuildingBlocks/Application/ICommandHandler.cs" -ItemType File
New-Item "$root/src/BuildingBlocks/Application/IQueryHandler.cs" -ItemType File

New-Item "$root/src/BuildingBlocks/Infrastructure/OutboxMessage.cs" -ItemType File
New-Item "$root/src/BuildingBlocks/Infrastructure/KafkaProducer.cs" -ItemType File

New-Item "$root/src/BuildingBlocks/Shared/Result.cs" -ItemType File

# ===========================================
# Create MODULE STRUCTURE
# ===========================================
Write-Host "Creating modules..." -ForegroundColor Green

foreach ($module in $modules) {
    Write-Host "Creating Module: $module"

    $moduleRoot = "$root/src/Modules/$module"
    New-Item -ItemType Directory -Force -Path $moduleRoot

    # Domain
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Domain"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Domain/Entities"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Domain/ValueObjects"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Domain/Events"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Domain/Aggregates"

    # Application
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Application"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Application/Commands"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Application/Queries"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Application/Handlers"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Application/DTOs"

    # Infrastructure
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Infrastructure"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Infrastructure/Repositories"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Infrastructure/Mappings"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Infrastructure/Migrations"

    # API
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Api"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Api/Controllers"
    New-Item -ItemType Directory -Force -Path "$moduleRoot/$module.Api/Requests"

    # Empty placeholder files
    New-Item "$moduleRoot/$module.Api/Controllers/${module}Controller.cs" -ItemType File
    New-Item "$moduleRoot/$module.Domain/Aggregates/${module}Root.cs" -ItemType File
}

# ===========================================
# Create solution & project files
# ===========================================
Write-Host "Creating solution..." -ForegroundColor Yellow

dotnet new sln -n $solutionName -o $root

# API Host
Write-Host "Creating ApiHost project..."
dotnet new webapi -n ApiHost -o "$root/src/ApiHost"
dotnet sln "$root/$solutionName.sln" add "$root/src/ApiHost/ApiHost.csproj"

# ===========================================
# Create Module Project Files
# ===========================================
foreach ($module in $modules) {
    $moduleRoot = "$root/src/Modules/$module"

    Write-Host "Creating csproj for $module..."

    dotnet new classlib -n "$module.Domain" -o "$moduleRoot/$module.Domain"
    dotnet new classlib -n "$module.Application" -o "$moduleRoot/$module.Application"
    dotnet new classlib -n "$module.Infrastructure" -o "$moduleRoot/$module.Infrastructure"
    dotnet new classlib -n "$module.Api" -o "$moduleRoot/$module.Api"

    # Add to solution
    dotnet sln "$root/$solutionName.sln" add "$moduleRoot/$module.Domain/$module.Domain.csproj"
    dotnet sln "$root/$solutionName.sln" add "$moduleRoot/$module.Application/$module.Application.csproj"
    dotnet sln "$root/$solutionName.sln" add "$moduleRoot/$module.Infrastructure/$module.Infrastructure.csproj"
    dotnet sln "$root/$solutionName.sln" add "$moduleRoot/$module.Api/$module.Api.csproj"

    # Add references
    dotnet add "$moduleRoot/$module.Application/$module.Application.csproj" reference "$moduleRoot/$module.Domain/$module.Domain.csproj"
    dotnet add "$moduleRoot/$module.Infrastructure/$module.Infrastructure.csproj" reference "$moduleRoot/$module.Domain/$module.Domain.csproj"
    dotnet add "$moduleRoot/$module.Api/$module.Api.csproj" reference "$moduleRoot/$module.Application/$module.Application.csproj"
}

Write-Host "DONE! Full project structure created successfully." -ForegroundColor Green
