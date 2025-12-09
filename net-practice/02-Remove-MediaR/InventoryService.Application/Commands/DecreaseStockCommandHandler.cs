using InventoryService.Application.Dispatch;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands
{
    public class DecreaseStockCommandHandler : ICommandHandler<DecreaseStockCommand>
    {
        private readonly IStockRepository _repo;

        public DecreaseStockCommandHandler(IStockRepository repo)
        {
            _repo = repo;
        }

        public async Task HandleAsync(DecreaseStockCommand command, CancellationToken ct = default)
        {
            var item = await _repo.GetByIdAsync(command.ItemId, ct)
                       ?? throw new KeyNotFoundException("Item not found");

            item.Decrease(command.Amount);

            await _repo.UpdateAsync(item, ct);
            // note: Do not call SaveChanges here; UoW will commit later
        }
    }
}
