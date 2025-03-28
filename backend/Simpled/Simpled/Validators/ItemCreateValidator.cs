using FluentValidation;
using Simpled.Dtos.Items;

namespace Simpled.Validators
{
    public class ItemCreateValidator : AbstractValidator<ItemCreateDto>
    {
        public ItemCreateValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.ColumnId).NotEmpty();
        }
    }
}
