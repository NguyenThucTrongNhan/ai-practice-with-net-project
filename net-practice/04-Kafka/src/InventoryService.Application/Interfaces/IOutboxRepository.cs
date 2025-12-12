using InventoryService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IOutboxRepository
    {
        Task AddAsync(OutboxMessageDto message, CancellationToken ct = default);
        Task<List<OutboxMessageDto>> GetUnprocessedAsync(int batchSize = 50, CancellationToken ct = default);
        Task MarkAsProcessedAsync(OutboxMessageDto message, CancellationToken ct = default);
        Task MarkAsFailedAsync(Guid id, string error, CancellationToken ct = default);
    }
}
