using InventoryService.Application;
using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Kafka;
using InventoryService.Infrastructure.Outbox.Repositories;
using InventoryService.Infrastructure.Outbox.Services;
using InventoryService.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Domain.Events;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(DecreaseStockCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LowStockEvent).Assembly);
});
// Application infra registrations
builder.Services.AddScoped<IStockRepository, StockRepository>(); // assume exists
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<OutboxPublisher>();
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddHostedService<OutboxProcessorHostedService>();

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
    var db = scope.ServiceProvider.GetRequiredService<InventoryService.Infrastructure.Persistence.InventoryDbContext>();
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
