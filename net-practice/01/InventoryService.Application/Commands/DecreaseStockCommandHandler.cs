using InventoryService.Application.Interfaces;
using InventoryService.Application.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Commands
{
    public class DecreaseStockCommandHandler : IRequestHandler<DecreaseStockCommand, Unit>
    {
        private readonly IStockRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public DecreaseStockCommandHandler(
            IStockRepository repo,
            IUnitOfWork uow,
            IDomainEventDispatcher domainEventDispatcher)
        {
            _repo = repo;
            _uow = uow;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<Unit> Handle(DecreaseStockCommand request, CancellationToken cancellationToken)
        {
            var item = await _repo.GetByIdAsync(request.ItemId, cancellationToken)
                       ?? throw new KeyNotFoundException("Item not found");

            item.Decrease(request.Amount);

            await _repo.UpdateAsync(item, cancellationToken);

            // Persist changes (Unit of Work)
            await _uow.SaveChangesAsync(cancellationToken);

            // After saving to DB, dispatch domain events
            await _domainEventDispatcher.DispatchAndClearEventsAsync(item, cancellationToken);

            return Unit.Value;
        }
    }
}
