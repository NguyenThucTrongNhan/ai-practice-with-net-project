using FluentValidation;
using InventoryService.Application.Commands;
using InventoryService.Application.Dispatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Application.Validators;

public class DecreaseStockCommandValidator : AbstractValidator<DecreaseStockCommand>, ICommandValidator<DecreaseStockCommand>
{
    public DecreaseStockCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }

    // ICommandValidator implemented by AbstractValidator via extension. We'll use wrapper in DI.
    public Task ValidateAsync(DecreaseStockCommand command, CancellationToken ct = default)
    {
        var res = Validate(command);
        if (!res.IsValid) throw new ValidationException(res.Errors);
        return Task.CompletedTask;
    }
}
