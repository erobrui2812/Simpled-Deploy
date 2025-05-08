using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Comments;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/items/{itemId:guid}/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;

        public CommentsController(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Obtiene todos los comentarios de un ítem.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetComments(Guid itemId)
        {
            var comments = await _commentRepo.GetByItemIdAsync(itemId);
            return Ok(comments);
        }

        /// <summary>
        /// Crea un nuevo comentario.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddComment(Guid itemId, [FromBody] CommentCreateDto dto)
        {
            if (itemId != dto.ItemId)
                return BadRequest("El ID del ítem no coincide.");

            var result = await _commentRepo.CreateAsync(CurrentUserId, dto);
            return CreatedAtAction(nameof(GetComments), new { itemId }, result);
        }

        /// <summary>
        /// Elimina un comentario (solo el autor).
        /// </summary>
        [HttpDelete("{commentId:guid}")]
        public async Task<IActionResult> Delete(Guid itemId, Guid commentId)
        {
            var success = await _commentRepo.DeleteAsync(commentId, CurrentUserId);
            return success ? NoContent() : Forbid("No puedes eliminar este comentario.");
        }

        /// <summary>
        /// Marca un comentario como resuelto/no resuelto.
        /// </summary>
        [HttpPatch("{commentId:guid}/resolve")]
        public async Task<IActionResult> ToggleResolved(Guid itemId, Guid commentId, [FromQuery] bool resolved)
        {
            var success = await _commentRepo.MarkAsResolvedAsync(commentId, CurrentUserId, resolved);
            return success ? NoContent() : Forbid("No puedes modificar este comentario.");
        }

        [HttpPut("{commentId:guid}")]
        public async Task<IActionResult> Update(
        Guid itemId,
        Guid commentId,
        [FromBody] CommentUpdateDto dto)
        {
            if (itemId != dto.ItemId)
                return BadRequest("El ID del ítem no coincide.");

            var updated = await _commentRepo.UpdateAsync(CurrentUserId, commentId, dto);
            return updated is not null
                ? Ok(updated)
                : Forbid("No puedes editar este comentario.");
        }
    }
}
