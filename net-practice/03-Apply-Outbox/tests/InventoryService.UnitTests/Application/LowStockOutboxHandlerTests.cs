using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using InventoryService.Application.EventHandlers;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Events;
using InventoryService.Application.Interfaces;

namespace InventoryService.UnitTests.Application;

public class LowStockOutboxHandlerTests
{
    [Fact]
    public async Task Handle_ShouldWriteOutboxItem_WhenLowStockEventRaised()
    {
        // Arrange
        var outboxRepoMock = new Mock<IOutboxRepository>();
        outboxRepoMock.Setup(r => r.AddAsync(It.IsAny<OutboxMessageDto>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask)
                      .Verifiable();

        var handler = new LowStockOutboxHandler(outboxRepoMock.Object);

        var evt = new LowStockEvent(System.Guid.NewGuid(), OldStock: 10, NewStock: 4);

        // Act
        await handler.Handle(evt, CancellationToken.None);

        // Assert
        outboxRepoMock.Verify(r => r.AddAsync(It.Is<OutboxMessageDto>(o =>
            o.EventType == nameof(LowStockEvent) && !string.IsNullOrWhiteSpace(o.Payload)
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
