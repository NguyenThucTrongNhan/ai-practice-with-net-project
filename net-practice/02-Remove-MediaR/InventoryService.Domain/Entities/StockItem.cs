using InventoryService.Domain.Common;
using InventoryService.Domain.Events;

namespace InventoryService.Domain.Entities
{
    public class StockItem : AggregateRoot
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;
        public int Quantity { get; private set; }

        public StockItem() { } // EF ctor

        public StockItem(string name, int initialQty)
        {
            Id = Guid.NewGuid();
            Name = name;
            Quantity = initialQty;
        }

        public void Decrease(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
            if (amount > Quantity) throw new InvalidOperationException("Not enough stock");

            Quantity -= amount;

            // Raise domain event when low
            if (Quantity < 10)
            {
                AddDomainEvent(new LowStockEvent(Id, Name, Quantity));
            }
        }
        public void Increase(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive", nameof(amount));
            Quantity += amount;
        }
    }
}
