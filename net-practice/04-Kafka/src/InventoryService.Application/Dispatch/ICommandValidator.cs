using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Dispatch;

public interface ICommandValidator<TCommand>
{
    Task ValidateAsync(TCommand command, CancellationToken ct = default);
}
