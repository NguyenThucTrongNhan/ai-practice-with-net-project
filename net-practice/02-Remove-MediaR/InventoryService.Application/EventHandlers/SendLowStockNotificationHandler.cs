using InventoryService.Application.Interfaces;
using InventoryService.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.EventHandlers
{
    public class SendLowStockNotificationHandler : INotificationHandler<LowStockEvent>
    {
        private readonly IEmailService _email;

        public SendLowStockNotificationHandler(IEmailService email)
        {
            _email = email;
        }

        public async Task Handle(LowStockEvent notification, CancellationToken cancellationToken)
        {
            var subject = $"Low stock alert - {notification.ItemName}";
            var body = $"Item: {notification.ItemName} (Id: {notification.ItemId}) has low stock: {notification.Quantity}";

            // In version 1 this is synchronous call to internal email service (console or simple SMTP wrapper)
            await _email.SendAsync("admin@company.local", subject, body, cancellationToken);
        }
    }
}
