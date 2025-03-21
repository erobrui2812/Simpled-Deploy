using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskBoard.api.Data;
using TaskBoard.api.Models.Dtos.BoardDtos;
using AutoMapper;

namespace TaskBoard.api.Controllers
{
    [ApiController]
    [Route("api/boards")]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BoardController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard([FromBody] BoardCreateDto dto)
        {
            var board = _mapper.Map<Board>(dto);
            board.OwnerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<BoardResponseDto>(board);
            return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, responseDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BoardResponseDto>> GetBoard(Guid id)
        {
            var board = await _context.Boards
                .Include(b => b.Columns)
                .ThenInclude(c => c.Items)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (board == null) return NotFound();

            return _mapper.Map<BoardResponseDto>(board);
        }
    }
}


