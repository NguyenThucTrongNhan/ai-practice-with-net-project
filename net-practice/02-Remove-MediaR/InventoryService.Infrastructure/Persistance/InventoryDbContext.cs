using InventoryService.Application.Interfaces;
using InventoryService.Domain.Common;
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Persistance
{
    public class InventoryDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventDispatcher _dispatcher;
        public InventoryDbContext(
            DbContextOptions<InventoryDbContext> options,
            IDomainEventDispatcher dispatcher)
            : base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<StockItem> StockItems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockItem>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.Quantity).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // 1️⃣ Collect domain events BEFORE saving
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // 2️⃣ Save changes to DB
            var result = await base.SaveChangesAsync(ct);

            // 3️⃣ Clear domain events AFTER commit
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                entry.Entity.ClearDomainEvents();
            }

            // 4️⃣ Dispatch collected events
            await _dispatcher.DispatchAllAsync(domainEvents, ct);

            return result;
        }
    }
}
