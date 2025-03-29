using FluentValidation;
using Simpled.Dtos.Boards;

namespace Simpled.Validators
{
    public class BoardCreateValidator : AbstractValidator<BoardCreateDto>
    {
        public BoardCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);


        }
    }
}
