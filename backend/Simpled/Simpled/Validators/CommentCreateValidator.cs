using FluentValidation;
using Simpled.Dtos.Comments;

namespace Simpled.Validators
{
    public class CommentCreateValidator : AbstractValidator<CommentCreateDto>
    {
        public CommentCreateValidator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("El comentario no puede estar vacío.");
            RuleFor(x => x.ItemId).NotEmpty().WithMessage("El ID del ítem es obligatorio.");
        }
    }
} 