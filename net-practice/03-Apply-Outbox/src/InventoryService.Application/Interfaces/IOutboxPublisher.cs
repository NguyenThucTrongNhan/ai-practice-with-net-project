using InventoryService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IOutboxPublisher
    {
        //Task PublishAsync(OutboxMessageDto message, CancellationToken ct = default);
        Task ProcessPendingAsync(int batchSize = 50, CancellationToken ct = default);
    }
}
