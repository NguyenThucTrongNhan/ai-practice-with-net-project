using Microsoft.AspNetCore.Mvc;
using MediatR;
using InventoryService.Application.Commands;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{id:guid}/decrease")]
    public async Task<IActionResult> Decrease(Guid id, [FromBody] DecreaseRequest request)
    {
        await _mediator.Send(new DecreaseStockCommand(id, request.Amount));
        return Ok();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateStockRequest req, [FromServices] InventoryService.Infrastructure.Persistance.InventoryDbContext db)
    {
        var item = new InventoryService.Domain.Entities.StockItem(req.Name, req.Quantity);
        db.StockItems.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, [FromServices] InventoryService.Infrastructure.Persistance.InventoryDbContext db)
    {
        var item = await db.StockItems.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    public record DecreaseRequest(int Amount);
    public record CreateStockRequest(string Name, int Quantity);
}
