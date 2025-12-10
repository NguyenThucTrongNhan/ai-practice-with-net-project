using InventoryService.Application.Interfaces;
using InventoryService.Domain.Common;
using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Outbox.Configurations;
using InventoryService.Infrastructure.Outbox.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Persistence
{
    public class InventoryDbContext : DbContext
    {
        private readonly IMediator _mediator;
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
        {
        }
        public InventoryDbContext(
            DbContextOptions<InventoryDbContext> options,
            IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<StockItem> StockItems { get; set; } = default!;
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockItem>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.Quantity).IsRequired();
            });
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        //public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        //{
        //    // 1️⃣ Collect domain events BEFORE saving
        //    var domainEvents = ChangeTracker
        //        .Entries<BaseEntity>()
        //        .SelectMany(x => x.Entity.DomainEvents)
        //        .ToList();

        //    // 2️⃣ Save changes to DB
        //    var result = await base.SaveChangesAsync(ct);

        //    // 3️⃣ Clear domain events AFTER commit
        //    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        //    {
        //        entry.Entity.ClearDomainEvents();
        //    }

        //    // 4️⃣ Dispatch collected events
        //    await _dispatcher.DispatchAllAsync(domainEvents, ct);

        //    return result;
        //}
        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // 1) Save aggregate changes first
            var result = await base.SaveChangesAsync(ct);

            // 2) After commit, collect domain events and publish via MediatR
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Clear domain events on entities (so they won't be re-dispatched)
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
                entry.Entity.ClearDomainEvents();

            foreach (var evt in domainEvents)
            {
                if (evt is INotification n)
                {
                    await _mediator.Publish(n, ct);
                }
            }

            return result;
        }
    }
}
