using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using InventoryService.Infrastructure.Outbox.Repositories;
using InventoryService.Infrastructure.Outbox.Entities;
using InventoryService.Infrastructure.Persistence;
using MediatR;
using InventoryService.Application.DTOs;

namespace InventoryService.UnitTests.Infrastructure;

public class OutboxRepositoryTests
{
    private InventoryDbContext CreateInMemoryDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        // create mediator mock as AppDbContext constructor may require it
        var mediatorMock = new Moq.Mock<IMediator>().Object; // will not be used in repo tests
        return new InventoryDbContext(options, mediatorMock);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertOutboxMessage()
    {
        var db = CreateInMemoryDb(Guid.NewGuid().ToString());
        var repo = new InventoryService.Infrastructure.Outbox.Repositories.OutboxRepository(db);

        var item = new OutboxMessageDto
        {
            Id = Guid.NewGuid(),
            EventType = "TestEvent",
            Payload = "{\"foo\":\"bar\"}",
            OccurredOn = DateTime.UtcNow
        };

        await repo.AddAsync(item);
        await db.SaveChangesAsync();

        var stored = db.OutboxMessages.SingleOrDefault(x => x.EventType == "TestEvent");
        stored.Should().NotBeNull();
        stored.Payload.Should().Contain("foo");
    }

    [Fact]
    public async Task GetUnprocessedAsync_ShouldReturnOnlyUnprocessed()
    {
        var db = CreateInMemoryDb(Guid.NewGuid().ToString());
        db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = "E1",
            Payload = "{}",
            OccurredOn = DateTime.UtcNow,
            ProcessedOn = null
        });

        db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = "E2",
            Payload = "{}",
            OccurredOn = DateTime.UtcNow,
            ProcessedOn = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var repo = new InventoryService.Infrastructure.Outbox.Repositories.OutboxRepository(db);
        var list = await repo.GetUnprocessedAsync(10);

        list.Should().HaveCount(1);
        list.First().EventType.Should().Be("E1");
    }

    [Fact]
    public async Task MarkAsProcessedAsync_ShouldSetProcessedOn()
    {
        var db = CreateInMemoryDb(Guid.NewGuid().ToString());
        var id = Guid.NewGuid();
        var outboxMessage = new OutboxMessage
        {
            Id = id,
            EventType = "E1",
            Payload = "{}",
            OccurredOn = DateTime.UtcNow,
            ProcessedOn = null
        };
        var outBoxDto = new OutboxMessageDto
        {
            Id = id,
            EventType = "E1",
            Payload = "{}",
            OccurredOn = DateTime.UtcNow,
        };
        db.OutboxMessages.Add(outboxMessage);
        await db.SaveChangesAsync();

        var repo = new OutboxRepository(db);
        await repo.MarkAsProcessedAsync(outBoxDto);
        await db.SaveChangesAsync();

        var e = db.OutboxMessages.Single(x => x.Id == id);
        e.ProcessedOn.Should().NotBeNull();
    }
}
