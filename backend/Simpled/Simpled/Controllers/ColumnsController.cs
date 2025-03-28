using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simpled.Dtos.Columns;
using Simpled.Repository;

namespace Simpled.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnRepository _columnService;

        public ColumnsController(IColumnRepository columnService)
        {
            _columnService = columnService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllColumns()
        {
            var result = await _columnService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColumn(Guid id)
        {
            var column = await _columnService.GetByIdAsync(id);
            return column == null ? NotFound("Column not found.") : Ok(column);
        }

        [HttpPost]
        public async Task<IActionResult> CreateColumn([FromBody] BoardColumnCreateDto dto)
        {
            var created = await _columnService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetColumn), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateColumn(Guid id, [FromBody] BoardColumnUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            var success = await _columnService.UpdateAsync(dto);
            return success ? NoContent() : NotFound("Column not found.");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColumn(Guid id)
        {
            var success = await _columnService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Column not found.");
        }
    }
}
