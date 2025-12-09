using InventoryService.Application.Interfaces;
using InventoryService.Domain.Events;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.EventHandlers;

public class SendLowStockNotificationHandler : IDomainEventHandler<LowStockEvent>
{
    private readonly ILogger<SendLowStockNotificationHandler> _logger;

    public SendLowStockNotificationHandler(
        ILogger<SendLowStockNotificationHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(LowStockEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "LOW STOCK WARNING → ProductId: {ProductId}, CurrentStock: {Stock}, OccurredOn: {OccurredOn}",
            domainEvent.ProductId,
            domainEvent.CurrentStock,
            domainEvent.OccurredOn
        );

        // 👉 TODO: Add email/SMS/notification/message bus publishing here

        return Task.CompletedTask;
    }
}
