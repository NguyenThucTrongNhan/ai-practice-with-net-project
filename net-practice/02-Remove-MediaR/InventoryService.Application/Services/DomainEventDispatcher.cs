using InventoryService.Application.Interfaces;
using InventoryService.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InventoryService.Application.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _services;

        public DomainEventDispatcher(IServiceProvider services)
        {
            _services = services;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
        {
            // Resolve handlers for this specific event type
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _services.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                await (Task)method.Invoke(handler, new object[] { domainEvent, ct });
            }
        }

        public async Task DispatchAllAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
        {
            foreach (var evt in events)
                await DispatchAsync(evt, ct);
        }
    }
}
