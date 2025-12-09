using Microsoft.AspNetCore.Mvc;
using InventoryService.Application.Commands;
using InventoryService.Application.Dispatch;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public StockController(ICommandDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpPost("{id:guid}/decrease")]
    public async Task<IActionResult> Decrease(Guid id, [FromBody] DecreaseRequest req, CancellationToken ct)
    {
        var cmd = new DecreaseStockCommand(id, req.Amount);
        await _dispatcher.DispatchAsync(cmd, ct);
        return Ok();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRequest req, [FromServices] InventoryService.Infrastructure.Persistance.InventoryDbContext db, CancellationToken ct)
    {
        var item = new InventoryService.Domain.Entities.StockItem(req.Name, req.Quantity);
        db.StockItems.Add(item);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, [FromServices] InventoryService.Infrastructure.Persistance.InventoryDbContext db, CancellationToken ct)
    {
        var item = await db.StockItems.FindAsync(new object[] { id }, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    public record DecreaseRequest(int Amount);
    public record CreateRequest(string Name, int Quantity);
}