using FluentValidation;
using Simpled.Dtos.Comments;

namespace Simpled.Validators
{
    public class CommentUpdateValidator : AbstractValidator<CommentUpdateDto>
    {
        public CommentUpdateValidator()
        {
            RuleFor(x => x.Text).NotEmpty().WithMessage("El comentario no puede estar vacío.");
            RuleFor(x => x.CommentId).NotEmpty().WithMessage("El ID del comentario es obligatorio.");
        }
    }
} 