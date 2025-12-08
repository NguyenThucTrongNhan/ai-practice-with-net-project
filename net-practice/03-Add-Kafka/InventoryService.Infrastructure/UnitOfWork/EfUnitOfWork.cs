using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.Persistance;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly InventoryDbContext _db;
        private readonly ILogger<EfUnitOfWork> _logger;

        public EfUnitOfWork(InventoryDbContext db, ILogger<EfUnitOfWork> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // Here you can implement transaction, retry, audit, etc.
            return await _db.SaveChangesAsync(ct);
        }
    }
}
