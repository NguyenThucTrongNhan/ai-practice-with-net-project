using InventoryService.Application;
using InventoryService.Application.Commands;
using InventoryService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure (DbContext, Repos, Email)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

// OpenTelemetry Tracing (simple)
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("ApiService"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("Application")              // Custom for CQRS or domain dispatcher
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
            });
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryService.Infrastructure.Persistance.InventoryDbContext>();
    if (!db.StockItems.Any())
    {
        db.StockItems.Add(new InventoryService.Domain.Entities.StockItem("Widget A", 15));
        db.StockItems.Add(new InventoryService.Domain.Entities.StockItem("Gadget B", 12));
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
