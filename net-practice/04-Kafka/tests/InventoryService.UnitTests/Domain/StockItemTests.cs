using FluentAssertions;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.UnitTests.Domain
{
    public class StockItemTests
    {
        [Fact]
        public void Decrease_ShouldReduceQuantity_AndRaiseLowStockEventWhenBelowThreshold()
        {
            // Arrange
            var item = new StockItem("Widget", 12);
            // ensure threshold is default 10 in your domain class; to test boundary set initial qty appropriately
            // Act
            item.Decrease(3); // 12 -> 9, crosses threshold 10

            // Assert
            item.Quantity.Should().Be(9);
            item.DomainEvents.Should().NotBeNull();
            item.DomainEvents.Should().ContainSingle(e => e is LowStockEvent);
            var ev = item.DomainEvents.First() as LowStockEvent;
            ev!.NewStock.Should().Be(9);
        }

        [Fact]
        public void Decrease_WhenAmountGreaterThanQuantity_ShouldThrow()
        {
            var item = new StockItem("Gizmo", 3);

            FluentActions.Invoking(() => item.Decrease(5))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Increase_ShouldIncreaseQuantity()
        {
            var item = new StockItem("Test", 5);
            item.Increase(2);
            item.Quantity.Should().Be(7);
        }
    }
}
