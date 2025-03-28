using FluentValidation;
using Simpled.Dtos.Columns;

namespace Simpled.Validators
{
    public class ColumnCreateValidator : AbstractValidator<BoardColumnCreateDto>
    {
        public ColumnCreateValidator()
        {
            RuleFor(x => x.BoardId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        }
    }
}
