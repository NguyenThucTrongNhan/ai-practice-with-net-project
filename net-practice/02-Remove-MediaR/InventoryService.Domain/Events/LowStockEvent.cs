
namespace InventoryService.Domain.Events;

public record LowStockEvent(Guid ProductId, string Name, int CurrentStock) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
