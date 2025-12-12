using Confluent.Kafka;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(IConfiguration cfg, ILogger<KafkaProducerService> logger)
        {
            //We also should create a part config for only producer
            _logger = logger;
            var kakConfig = new ProducerConfig
            {
                BootstrapServers = cfg["Kafka:BootstrapServers"] ?? "localhost:9092",
                Acks = Acks.All,
                EnableIdempotence = true
            };
            _producer = new ProducerBuilder<Null, string>(kakConfig).Build();
        }

        public async Task PublishAsync(string topic, string payload, CancellationToken ct = default)
        {
            var msg = new Message<Null, string> { Value = payload };
            var r = await _producer.ProduceAsync(topic, msg, ct);
            _logger.LogInformation("Kafka produced to {Topic}/{Partition}@{Offset}", r.Topic, r.Partition, r.Offset);
        }

        public void Dispose() => _producer?.Dispose();

    }
}
