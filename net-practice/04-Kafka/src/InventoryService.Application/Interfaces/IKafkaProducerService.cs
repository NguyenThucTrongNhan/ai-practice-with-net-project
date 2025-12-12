using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IKafkaProducerService
    {
        Task PublishAsync(string topic, string payload, CancellationToken ct = default);
    }
}
