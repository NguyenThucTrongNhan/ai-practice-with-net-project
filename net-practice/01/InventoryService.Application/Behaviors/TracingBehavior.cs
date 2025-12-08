using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Behaviors
{
    public class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
    {
        private static readonly ActivitySource ActivitySource = new("InventoryService.Application");

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var name = typeof(TRequest).Name;
            using var activity = ActivitySource.StartActivity(name);
            activity?.SetTag("messaging.system", "mediatr");
            activity?.SetTag("messaging.request", name);

            return await next();
        }
    }
}
