using InventoryService.Infrastructure.Outbox.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Outbox
{
    public class OutboxProcessorHostedService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<OutboxProcessorHostedService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

        public OutboxProcessorHostedService(IServiceProvider provider, 
            ILogger<OutboxProcessorHostedService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processor starting");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisher>();
                    await publisher.ProcessPendingAsync(50, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox processor error");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
