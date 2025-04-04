using FluentValidation;
using Simpled.Dtos.Boards;

namespace Simpled.Validators
{
    public class BoardUpdateValidator : AbstractValidator<BoardUpdateDto>
    {
        public BoardUpdateValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

          
        }
    }
}
