using Simpled.Repository;
using System.Security.Claims;


namespace Simpled.Helpers
{
    public static class BoardAuthorizationHelper
    {
        /// <summary>
        /// Verifica si el usuario autenticado tiene uno de los roles permitidos en un board específico.
        /// </summary>
        /// <param name="user">ClaimsPrincipal del usuario autenticado</param>
        /// <param name="boardId">ID del tablero</param>
        /// <param name="allowedRoles">Lista de roles válidos ("admin", "editor", "viewer")</param>
        /// <param name="boardMemberRepo">Repositorio de miembros de tableros</param>
        /// <returns>True si el usuario tiene permiso, False si no</returns>
        public static async Task<bool> HasBoardPermissionAsync(
            ClaimsPrincipal user,
            Guid boardId,
            string[] allowedRoles,
            IBoardMemberRepository boardMemberRepo)
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return false;

            if (!Guid.TryParse(userIdStr, out Guid userId))
                return false;

            var member = await boardMemberRepo.GetByIdsAsync(boardId, userId);
            if (member == null)
                return false;

            return allowedRoles.Contains(member.Role.ToLower());
        }
    }
}
