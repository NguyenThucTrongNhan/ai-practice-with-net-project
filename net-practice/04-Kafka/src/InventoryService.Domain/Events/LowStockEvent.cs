
using MediatR;

namespace InventoryService.Domain.Events;

public record LowStockEvent(Guid ProductId, int OldStock, int NewStock) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
