using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly InventoryDbContext _db;

        public StockRepository(InventoryDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(StockItem item, CancellationToken ct = default)
        {
            await _db.StockItems.AddAsync(item, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<StockItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.StockItems.FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task UpdateAsync(StockItem item, CancellationToken ct = default)
        {
            _db.StockItems.Update(item);
            await _db.SaveChangesAsync(ct);
        }
    }
}
