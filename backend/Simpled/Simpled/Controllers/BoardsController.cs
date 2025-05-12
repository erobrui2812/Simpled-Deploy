    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Simpled.Dtos.Boards;
    using Simpled.Helpers;
    using Simpled.Repository;
    using System.Security.Claims;

    namespace Simpled.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class BoardsController : ControllerBase
        {
            private readonly IBoardRepository _boardService;
            private readonly IBoardMemberRepository _boardMemberRepo;

            public BoardsController(IBoardRepository boardService, IBoardMemberRepository boardMemberRepo)
            {
                _boardService = boardService;
                _boardMemberRepo = boardMemberRepo;
            }

            /// <summary>
            /// Obtiene todos los tableros visibles para el usuario (públicos o propios).
            /// </summary>
            [HttpGet]
            public async Task<IActionResult> GetAllBoards()
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Guid? userId = string.IsNullOrEmpty(userIdClaim) ? null : Guid.Parse(userIdClaim);

                var result = await _boardService.GetAllAsync(userId);
                return Ok(result);
            }

            /// <summary>
            /// Obtiene un tablero específico por ID.
            /// </summary>
            [HttpGet("{id}")]
            public async Task<IActionResult> GetBoard(Guid id)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("No se pudo identificar al usuario.");

                Guid userId = Guid.Parse(userIdClaim);
                var board = await _boardService.GetByIdAsync(id, userId);
                return board == null ? NotFound("No se ha encontrado el tablero.") : Ok(board);
            }

            /// <summary>
            /// Crea un nuevo tablero.
            /// </summary>
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> CreateBoard([FromBody] BoardCreateDto dto)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("No se pudo identificar al usuario.");

                var created = await _boardService.CreateAsync(dto, Guid.Parse(userId));
                return CreatedAtAction(nameof(GetBoard), new { id = created.Id }, created);
            }

            /// <summary>
            /// Actualiza un tablero existente.
            /// </summary>
            [Authorize]
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] BoardUpdateDto dto)
            {
                if (id != dto.Id)
                    return BadRequest("ID mismatch.");

                var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, dto.Id, new[] { "admin" }, _boardMemberRepo);

                if (!hasPermission)
                    return Forbid("No tienes permisos para modificar este tablero.");

                var success = await _boardService.UpdateAsync(dto);
                return success ? NoContent() : NotFound("Board not found.");
            }

            /// <summary>
            /// Elimina un tablero por ID.
            /// </summary>
            [Authorize]
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteBoard(Guid id)
            {
                var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                    User, id, new[] { "admin" }, _boardMemberRepo);

                if (!hasPermission)
                    return Forbid("No tienes permisos para eliminar este tablero.");

                var success = await _boardService.DeleteAsync(id);
                return success ? NoContent() : NotFound("Board not found.");
            }
        }
    }
