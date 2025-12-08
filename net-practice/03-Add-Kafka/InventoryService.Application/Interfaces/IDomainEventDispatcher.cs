using InventoryService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEventsAsync(AggregateRoot aggregateRoot, CancellationToken ct = default);
    }
}
