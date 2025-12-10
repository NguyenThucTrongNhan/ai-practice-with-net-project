using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Services
{
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            // Very simple: log to console. Replace with SMTP/SendGrid in prod.
            _logger.LogInformation("Sending email to {To} | Subject: {Subject} | Body: {Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}
