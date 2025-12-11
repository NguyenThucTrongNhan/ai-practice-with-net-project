using InventoryService.Domain.Common;
using InventoryService.Domain.Events;

namespace InventoryService.Domain.Entities
{
    public class StockItem : AggregateRoot
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public int Quantity { get; private set; }
        public int LowStockThreshold { get; private set; } = 10;
        public StockItem() { } // EF ctor

        public StockItem(string name, int initialQty)
        {
            Id = Guid.NewGuid();
            Name = name;
            Quantity = initialQty;
        }

        public void Decrease(int amount)
        {
            if (amount <= 0) throw new ArgumentException("amount>0");
            if (amount > Quantity) throw new InvalidOperationException("not enough stock");

            var old = Quantity;
            Quantity -= amount;

            if (Quantity < LowStockThreshold)
            {
                AddDomainEvent(new LowStockEvent(Id, old, Quantity));
            }
        }

        public void Increase(int amount)
        {
            if (amount <= 0) throw new ArgumentException("amount>0");
            Quantity += amount;
        }
    }
}
