using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.Outbox.Entities;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Outbox.Repositories
{
    public class OutboxRepository :IOutboxRepository
    {
        private readonly InventoryDbContext _db;

        public OutboxRepository(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(OutboxMessageDto item, CancellationToken ct = default)
        {
            var message = new OutboxMessage
            {
                Id = item.Id,
                EventType = item.EventType,
                Payload = item.Payload,
                OccurredOn = item.OccurredOn
            };

            await _db.Set<OutboxMessage>().AddAsync(message, ct);
        }

        public async Task<List<OutboxMessageDto>> GetUnprocessedAsync(int batchSize = 50, CancellationToken ct = default)
        {
            return await _db.Set<OutboxMessage>()
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .Take(batchSize)
                .Select(x => new OutboxMessageDto
                {
                    Id = x.Id,
                    EventType = x.EventType,
                    Payload = x.Payload,
                    OccurredOn = x.OccurredOn
                })
                .ToListAsync(ct);
        }

        public async Task MarkAsFailedAsync(Guid id, string error, CancellationToken ct = default)
        {
            var e = await _db.OutboxMessages.FindAsync(new object[] { id }, ct);
            if (e != null) e.Error = error;
        }

        public async Task MarkAsProcessedAsync(OutboxMessageDto message, CancellationToken ct = default)
        {
            var entity = await _db.Set<OutboxMessage>().FindAsync(new object[] { message.Id }, ct);

            if (entity is not null)
            {
                entity.ProcessedOn = DateTime.UtcNow;
            }
        }
    }
}
