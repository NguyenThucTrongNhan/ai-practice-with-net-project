using InventoryService.Domain.Common;
using InventoryService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IDomainEventDispatcher
    {
        //Task DispatchAndClearEventsAsync(AggregateRoot aggregateRoot, CancellationToken ct = default);
        Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default);
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default);
    }
}
