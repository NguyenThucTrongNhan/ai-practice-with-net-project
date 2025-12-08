using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Domain.Events
{
    public record LowStockEvent(Guid ItemId, string ItemName, int Quantity) : INotification;
}
