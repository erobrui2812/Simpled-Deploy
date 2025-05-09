using FluentValidation;
using Simpled.Dtos.BoardMembers;

namespace Simpled.Validators
{
    public class BoardMemberUpdateValidator : AbstractValidator<BoardMemberUpdateDto>
    {
        public BoardMemberUpdateValidator()
        {
            RuleFor(x => x.BoardId)
                .NotEmpty().WithMessage("El ID del tablero es obligatorio.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID del usuario es obligatorio.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol es obligatorio.")
                .Must(BeAValidRole)
                .WithMessage("El rol debe ser 'admin', 'editor' o 'viewer'.");
        }

        private static bool BeAValidRole(string role)
        {
            return new[] { "admin", "editor", "viewer" }.Contains(role.ToLower());
        }
    }
}

