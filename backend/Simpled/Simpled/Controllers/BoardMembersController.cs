using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.BoardMembers;
using Simpled.Repository;

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
        [HttpGet("board/{boardId}")]
        public async Task<IActionResult> GetByBoardId(Guid boardId)
        {
            var result = await _memberService.GetByBoardIdAsync(boardId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un miembro específico por BoardId y UserId.
        /// </summary>
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
        public async Task<IActionResult> Create([FromBody] object body)
        {
            if (body is null)
                return BadRequest("No se proporcionaron datos.");

            if (HttpContext.Request.ContentType != "application/json")
                return BadRequest("Formato de contenido inválido.");

            try
            {
                var singleDto = System.Text.Json.JsonSerializer.Deserialize<BoardMemberCreateDto>(body.ToString()!, new() { PropertyNameCaseInsensitive = true });
                if (singleDto != null && singleDto.BoardId != Guid.Empty)
                {
                    await _memberService.AddAsync(singleDto);
                    return Ok("Miembro agregado.");
                }
            }
            catch { }

            try
            {
                var listDto = System.Text.Json.JsonSerializer.Deserialize<List<BoardMemberCreateDto>>(body.ToString()!, new() { PropertyNameCaseInsensitive = true });
                if (listDto != null && listDto.Count > 0)
                {
                    await _memberService.AddManyAsync(listDto);
                    return Ok("Miembros agregados.");
                }
            }
            catch { }

            return BadRequest("Datos inválidos.");
        }

        /// <summary>
        /// Actualiza el rol de un miembro en un tablero.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BoardMemberUpdateDto dto)
        {
            var success = await _memberService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Miembro no encontrado.");
        }

        /// <summary>
        /// Elimina un miembro de un tablero.
        /// </summary>
        [HttpDelete("{boardId}/{userId}")]
        public async Task<IActionResult> Delete(Guid boardId, Guid userId)
        {
            var success = await _memberService.DeleteAsync(boardId, userId);
            return success ? NoContent() : NotFound("Miembro no encontrado.");
        }
    }
}
