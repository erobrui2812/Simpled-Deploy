using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Boards;
using Simpled.Repository;
using System.Security.Claims;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardRepository _boardService;

        public BoardsController(IBoardRepository boardService)
        {
            _boardService = boardService;
        }

        /// <summary>
        /// Obtiene todos los tableros disponibles.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBoards()
        {
            var result = await _boardService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un tablero específico por ID.
        /// </summary>
        /// <param name="id">ID del tablero</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBoard(Guid id)
        {
            var board = await _boardService.GetByIdAsync(id);
            return board == null ? NotFound("Board not found.") : Ok(board);
        }

        /// <summary>
        /// Crea un nuevo tablero.
        /// </summary>
        /// <param name="dto">Datos del tablero a crear</param>
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
        /// <param name="id">ID del tablero</param>
        /// <param name="dto">Datos del tablero a actualizar</param>
        [Authorize(Roles = "admin,editor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] BoardUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var success = await _boardService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Board not found.");
        }

        /// <summary>
        /// Elimina un tablero por ID.
        /// </summary>
        /// <param name="id">ID del tablero</param>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(Guid id)
        {
            var success = await _boardService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Board not found.");
        }
    }
}

