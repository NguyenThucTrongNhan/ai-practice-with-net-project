using InventoryService.Application.Interfaces;
using InventoryService.Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAndClearEventsAsync(AggregateRoot aggregateRoot, CancellationToken ct = default)
        {
            var events = aggregateRoot.DomainEvents.ToArray();
            foreach (var @event in events)
            {
                // publish via MediatR; handlers in Application will respond
                await _mediator.Publish(@event, ct);
            }

            aggregateRoot.ClearDomainEvents();
        }
    }
}
