using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.BoardMembers;
using Simpled.Helpers;
using Simpled.Repository;
using System.Text.Json;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardMembersController : ControllerBase
    {
        private readonly IBoardMemberRepository _memberService;

        public BoardMembersController(IBoardMemberRepository memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Obtiene todos los miembros de todos los tableros.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _memberService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene los miembros de un tablero específico.
        /// </summary>
        /// <param name="boardId">ID del tablero</param>
        [HttpGet("board/{boardId}")]
        public async Task<IActionResult> GetByBoardId(Guid boardId)
        {
            var result = await _memberService.GetByBoardIdAsync(boardId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un miembro específico por BoardId y UserId.
        /// </summary>
        /// <param name="boardId">ID del tablero</param>
        /// <param name="userId">ID del usuario</param>
        [HttpGet("{boardId}/{userId}")]
        public async Task<IActionResult> Get(Guid boardId, Guid userId)
        {
            var member = await _memberService.GetByIdsAsync(boardId, userId);
            return member == null ? NotFound("Miembro no encontrado.") : Ok(member);
        }

        /// <summary>
        /// Agrega uno o varios miembros a un tablero.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] JsonElement body)
        {
            Guid boardId;

            if (body.TryGetProperty("boardId", out var boardIdProp))
                boardId = boardIdProp.GetGuid();
            else if (body.ValueKind == JsonValueKind.Array)
            {
                var first = JsonDocument.Parse(body.GetRawText()).RootElement[0];
                boardId = first.GetProperty("boardId").GetGuid();
            }
            else
                return BadRequest("No se pudo determinar el BoardId.");

            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin" }, _memberService);

            if (!hasPermission)
                return Forbid("No tienes permisos para agregar miembros a este tablero.");

            if (body.ValueKind == JsonValueKind.Object)
            {
                var dto = JsonSerializer.Deserialize<BoardMemberCreateDto>(body.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dto != null)
                {
                    await _memberService.AddAsync(dto);
                    return Ok("Miembro agregado.");
                }
            }
            else if (body.ValueKind == JsonValueKind.Array)
            {
                var dtoList = JsonSerializer.Deserialize<List<BoardMemberCreateDto>>(body.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dtoList != null && dtoList.Any())
                {
                    await _memberService.AddManyAsync(dtoList);
                    return Ok("Miembros agregados.");
                }
            }

            return BadRequest("Formato de datos inválido.");
        }

        /// <summary>
        /// Actualiza el rol de un miembro en un tablero.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BoardMemberUpdateDto dto)
        {
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, dto.BoardId, new[] { "admin" }, _memberService);

            if (!hasPermission)
                return Forbid("No tienes permisos para actualizar este miembro.");

            var success = await _memberService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Miembro no encontrado.");
        }

        /// <summary>
        /// Elimina un miembro de un tablero.
        /// </summary>
        /// <param name="boardId">ID del tablero</param>
        /// <param name="userId">ID del usuario</param>
        [HttpDelete("{boardId}/{userId}")]
        public async Task<IActionResult> Delete(Guid boardId, Guid userId)
        {
            var hasPermission = await BoardAuthorizationHelper.HasBoardPermissionAsync(
                User, boardId, new[] { "admin" }, _memberService);

            if (!hasPermission)
                return Forbid("No tienes permisos para eliminar miembros de este tablero.");

            var success = await _memberService.DeleteAsync(boardId, userId);
            return success ? NoContent() : NotFound("Miembro no encontrado.");
        }
    }
}
