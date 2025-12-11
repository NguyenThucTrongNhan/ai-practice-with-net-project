# ================================================
# Full MCP Server MVP Project Generator (C# Clean Architecture + DDD)
# ================================================
$solutionName = "McpServer"
$root = "./$solutionName"

Write-Host "Creating solution folder: $root"
New-Item -ItemType Directory -Force -Path $root | Out-Null
Set-Location $root

# --------------------------------
# 1. Create Solution
# --------------------------------
Write-Host "Creating .sln ..."
dotnet new sln -n $solutionName

# --------------------------------
# 2. Create Projects
# --------------------------------
Write-Host "Creating projects ..."

# Core Domain
dotnet new classlib -n "$solutionName.Core.Domain" -o "src/Core/Domain"

# Core Application
dotnet new classlib -n "$solutionName.Core.Application" -o "src/Core/Application"

# Infrastructure
dotnet new classlib -n "$solutionName.Infrastructure" -o "src/Infrastructure"

# Presentation Host
dotnet new console -n "$solutionName.Presentation.McpServerHost" -o "src/Presentation/McpServerHost"

# --------------------------------
# 3. Add Projects to Solution
# --------------------------------
Write-Host "Adding projects to solution ..."

dotnet sln add "src/Core/Domain/$solutionName.Core.Domain.csproj"
dotnet sln add "src/Core/Application/$solutionName.Core.Application.csproj"
dotnet sln add "src/Infrastructure/$solutionName.Infrastructure.csproj"
dotnet sln add "src/Presentation/McpServerHost/$solutionName.Presentation.McpServerHost.csproj"

# --------------------------------
# 4. Add References (Clean Architecture)
# --------------------------------
Write-Host "Adding project references ..."

# Application depends on Domain
dotnet add "src/Core/Application/$solutionName.Core.Application.csproj" reference `
    "src/Core/Domain/$solutionName.Core.Domain.csproj"

# Infrastructure depends on Application + Domain
dotnet add "src/Infrastructure/$solutionName.Infrastructure.csproj" reference `
    "src/Core/Application/$solutionName.Core.Application.csproj"
dotnet add "src/Infrastructure/$solutionName.Infrastructure.csproj" reference `
    "src/Core/Domain/$solutionName.Core.Domain.csproj"

# Presentation depends on Infrastructure + Application
dotnet add "src/Presentation/McpServerHost/$solutionName.Presentation.McpServerHost.csproj" reference `
    "src/Infrastructure/$solutionName.Infrastructure.csproj"
dotnet add "src/Presentation/McpServerHost/$solutionName.Presentation.McpServerHost.csproj" reference `
    "src/Core/Application/$solutionName.Core.Application.csproj"

# --------------------------------
# 5. Create Additional Folders
# --------------------------------
Write-Host "Creating additional DDD folders ..."

$folders = @(
    "src/Core/Domain/Entities",
    "src/Core/Domain/ValueObjects",
    "src/Core/Domain/Interfaces",

    "src/Core/Application/Interfaces",
    "src/Core/Application/Services",
    "src/Core/Application/DTOs",

    "src/Infrastructure/McpProtocol",
    "src/Infrastructure/Serialization",
    "src/Infrastructure/Logging",

    "src/Presentation/McpServerHost/Config",
    "src/Presentation/McpServerHost/Tools",

    "tests/Core.Tests",
    "tests/Application.Tests",
    "tests/Infrastructure.Tests",
    "tests/EndToEnd.Tests"
)

foreach ($folder in $folders) {
    New-Item -ItemType Directory -Force -Path $folder | Out-Null
}

# --------------------------------
# 6. Create Empty Files
# --------------------------------
Write-Host "Creating placeholder files ..."

$files = @{
    # Domain layer
    "src/Core/Domain/Entities/ToolDefinition.cs" = "";
    "src/Core/Domain/ValueObjects/ToolArgument.cs" = "";
    "src/Core/Domain/Interfaces/ITool.cs" = "";

    # Application layer
    "src/Core/Application/Interfaces/IToolRegistry.cs" = "";
    "src/Core/Application/Interfaces/IMcpMessageProcessor.cs" = "";
    "src/Core/Application/Services/ToolRegistry.cs" = "";
    "src/Core/Application/DTOs/McpRequestDto.cs" = "";
    "src/Core/Application/DTOs/McpResponseDto.cs" = "";

    # Infrastructure layer
    "src/Infrastructure/McpProtocol/McpMessageLoop.cs" = "";
    "src/Infrastructure/McpProtocol/McpJsonRpcHandler.cs" = "";
    "src/Infrastructure/Serialization/JsonSerializerWrapper.cs" = "";
    "src/Infrastructure/Logging/ConsoleLogger.cs" = "";

    # Presentation layer
    "src/Presentation/McpServerHost/Program.cs" = "";
    "src/Presentation/McpServerHost/McpServer.cs" = "";
    "src/Presentation/McpServerHost/Tools/GetTimeTool.cs" = "";
    "src/Presentation/McpServerHost/Tools/EchoTool.cs" = "";
    "src/Presentation/McpServerHost/Config/appsettings.json" = "{}";
    "src/Presentation/McpServerHost/Config/mcp.config.json" = "{}";

    # Test projects
    "tests/Core.Tests/ToolDefinitionTests.cs" = "";
    "tests/Application.Tests/ToolRegistryTests.cs" = "";
    "tests/Infrastructure.Tests/McpJsonRpcHandlerTests.cs" = "";
    "tests/EndToEnd.Tests/McpServerEndToEndTests.cs" = "";
}

foreach ($file in $files.Keys) {
    New-Item -ItemType File -Force -Path $file | Out-Null
}

Write-Host "====================================================="
Write-Host "MCP Server MVP Clean Architecture solution is READY!"
Write-Host "Solution: $solutionName.sln"
Write-Host "====================================================="
