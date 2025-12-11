using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryService.Application.EventHandlers;

// This MediatR handler reacts to LowStockEvent (internal domain event) and writes an OutboxItem (integration event).
public class LowStockOutboxHandler : INotificationHandler<LowStockEvent>
{
    private readonly IOutboxRepository _outboxRepo;

    public LowStockOutboxHandler(IOutboxRepository outboxRepo)
    {
        _outboxRepo = outboxRepo;
    }

    public async Task Handle(LowStockEvent notification, CancellationToken ct)
    {
        var payload = new
        {
            ProductId = notification.ProductId,
            OldStock = notification.OldStock,
            NewStock = notification.NewStock,
            OccurredOn = notification.OccurredOn
        };

        var json = JsonSerializer.Serialize(payload);

        var outbox = new OutboxMessageDto
        {
            Id = Guid.NewGuid(),
            EventType = nameof(LowStockEvent),
            Payload = json,
            OccurredOn = DateTime.UtcNow
        };

        await _outboxRepo.AddAsync(outbox, ct);
        // Do not SaveChanges here — the SaveChanges that triggered MediatR publishing already committed domain changes.
        // We rely on AppDbContext SaveChanges to persist outbox rows if OutboxRepo adds directly to context (see infra).
    }
}
