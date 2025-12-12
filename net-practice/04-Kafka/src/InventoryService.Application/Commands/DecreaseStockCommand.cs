using MediatR;

namespace InventoryService.Application.Commands;

public record DecreaseStockCommand(Guid ItemId, int Amount) : IRequest<Unit>; 
