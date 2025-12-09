using FluentValidation;
using InventoryService.Application.Commands;
using InventoryService.Application.Dispatch;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register command dispatcher
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        // Register handlers (concrete)
        services.AddScoped<ICommandHandler<InventoryService.Application.Commands.DecreaseStockCommand>, DecreaseStockCommandHandler>();

        // Register validators from assembly (FluentValidation)
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);

        // If validators implement ICommandValidator<T>, register them automatically:
        // Fluent validators above implement ICommandValidator<T> directly in our sample.

        return services;
    }
}
