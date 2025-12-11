using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Outbox.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = default!;        // event type name
        public string Payload { get; set; } = default!;     // serialized JSON
        public DateTime? ProcessedOn { get; set; }
        public string? Error { get; set; }
    }
}
