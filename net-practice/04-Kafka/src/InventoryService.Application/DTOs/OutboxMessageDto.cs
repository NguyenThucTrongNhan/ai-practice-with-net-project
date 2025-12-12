using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.DTOs
{
    public class OutboxMessageDto
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}
