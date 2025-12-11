using InventoryService.Application.DTOs;
using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Outbox.Services
{
    public class OutboxPublisher : IOutboxPublisher
    {
        private readonly IKafkaProducerService _kafka;
        private readonly IOutboxRepository _repo;
        private readonly InventoryDbContext _db;

        public OutboxPublisher(IKafkaProducerService kafka, IOutboxRepository repo,
            InventoryDbContext db)
        {
            _kafka = kafka;
            _repo = repo;
            _db = db;

        }

        //public async Task PublishAsync(OutboxMessageDto item, CancellationToken ct = default)
        //{
        //    await _kafka.ProduceAsync(item.EventType, item.Payload);
        //}
        public async Task ProcessPendingAsync(int batchSize = 50, CancellationToken ct = default)
        {
            var pending = await _repo.GetUnprocessedAsync(batchSize, ct);
            foreach (var item in pending)
            {
                try
                {
                    // Map EventType -> topic. Here we use a simple naming convention
                    var topic = MapEventTypeToTopic(item.EventType);
                    await _kafka.PublishAsync(topic, item.Payload, ct);
                    await _repo.MarkAsProcessedAsync(item, ct);
                }
                catch (Exception ex)
                {
                    await _repo.MarkAsFailedAsync(item.Id, ex.Message, ct);
                }
            }

            await _db.SaveChangesAsync(ct);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private string MapEventTypeToTopic(string eventType)
        {
            // simple: "LowStockEvent" -> "inventory.lowstock"
            var name = eventType;
            if (name.EndsWith("Event")) name = name[..^"Event".Length];
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c) && i > 0) sb.Append('.');
                sb.Append(char.ToLowerInvariant(c));
            }
            return $"inventory.{sb}";
        }
    }
}
