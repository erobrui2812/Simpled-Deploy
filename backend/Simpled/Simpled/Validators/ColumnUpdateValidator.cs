using FluentValidation;
using Simpled.Dtos.Columns;

namespace Simpled.Validators
{
    public class ColumnUpdateValidator : AbstractValidator<BoardColumnUpdateDto>
    {
        public ColumnUpdateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.BoardId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        }
    }
}
