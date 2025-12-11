using InventoryService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Dispatch;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<CommandDispatcher> _logger;
    private static readonly ActivitySource ActivitySource = new("InventoryService.Application");

    public CommandDispatcher(IServiceProvider provider, ILogger<CommandDispatcher> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken ct = default)
    {
        var commandName = typeof(TCommand).Name;
        using var activity = ActivitySource.StartActivity(commandName);
        activity?.SetTag("command", commandName);

        _logger.LogInformation("Dispatching {Command} with payload: {@Payload}", commandName, command);

        // 1. Validation
        var validators = _provider.GetServices<ICommandValidator<TCommand>>().ToList();
        foreach (var validator in validators)
            await validator.ValidateAsync(command, ct);

        // 2. Resolve handler
        var handler = _provider.GetService<ICommandHandler<TCommand>>();
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {typeof(TCommand).FullName}");

        // 3. Execute handler
        await handler.HandleAsync(command, ct);

        // 4. Commit UoW — DbContext handles domain events internally
        var uow = _provider.GetService<IUnitOfWork>();
        if (uow != null)
            await uow.SaveChangesAsync(ct);

        _logger.LogInformation("Command {Command} handled successfully", commandName);
    }
}
