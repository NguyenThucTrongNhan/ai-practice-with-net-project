using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using InventoryService.Application.Commands;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using MediatR;

namespace InventoryService.UnitTests.Application;

public class DecreaseStockCommandHandlerTests
{
    [Fact]
    public async Task Handle_DecreasesStock_AndCallsSave()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var stockItem = new StockItem("Item A", 10);

        var repoMock = new Mock<IStockRepository>();
        repoMock.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockItem);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

        var mediatorMock = new Mock<IMediator>();

        var handler = new DecreaseStockCommandHandler(repoMock.Object, uowMock.Object, mediatorMock.Object);

        var command = new DecreaseStockCommand(itemId, 3);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        stockItem.Quantity.Should().Be(7);
        repoMock.Verify(r => r.UpdateAsync(stockItem, It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ThrowsKeyNotFoundException()
    {
        var repoMock = new Mock<IStockRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((StockItem?)null);

        var uowMock = new Mock<IUnitOfWork>();
        var mediatorMock = new Mock<IMediator>();

        var handler = new DecreaseStockCommandHandler(repoMock.Object, uowMock.Object, mediatorMock.Object);
        var cmd = new DecreaseStockCommand(Guid.NewGuid(), 1);

        await FluentActions.Invoking(() => handler.Handle(cmd, CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }
}
