using InventoryService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces
{
    public interface IStockRepository
    {
        Task<StockItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(StockItem item, CancellationToken ct = default);
        Task UpdateAsync(StockItem item, CancellationToken ct = default);
    }
}
