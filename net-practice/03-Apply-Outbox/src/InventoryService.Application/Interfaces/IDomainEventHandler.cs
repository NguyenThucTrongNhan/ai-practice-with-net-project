using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces;

public interface IDomainEventHandler<TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken ct = default);
}