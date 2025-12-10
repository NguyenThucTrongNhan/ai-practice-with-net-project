using InventoryService.Application.Dispatch;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Events;
using MediatR;

namespace InventoryService.Application.Commands
{
    public class DecreaseStockCommandHandler
        : IRequestHandler<DecreaseStockCommand, Unit>
    {
        private readonly IStockRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMediator _mediator;

        public DecreaseStockCommandHandler(
            IStockRepository repo,
            IUnitOfWork uow,
            IMediator mediator)
        {
            _repo = repo;
            _uow = uow;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DecreaseStockCommand request, CancellationToken ct)
        {
            var item = await _repo.GetByIdAsync(request.ItemId, ct)
                ?? throw new KeyNotFoundException("Item not found");

            // Domain logic
            item.Decrease(request.Amount);

            // If your repository uses EF Core tracking, this line is optional
            await _repo.UpdateAsync(item, ct);

            // SaveChanges → triggers domain events → handled by MediatR
            await _uow.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }

}
