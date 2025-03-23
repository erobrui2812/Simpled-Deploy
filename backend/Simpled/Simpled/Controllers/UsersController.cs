using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Models;
using Simpled.Dtos.Users;
using Microsoft.AspNetCore.Authorization;

namespace Simpled.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SimpledDbContext _context;

        public UsersController(SimpledDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        /// <summary>
        /// Devuelve todos los usuarios registrados.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetUsers()
        {
            var users = await _context.Users.Include(u => u.Roles).ToListAsync();


            var result = users.Select(u => new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                Roles = u.Roles.Select(r => r.Role).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/users/{id}
        /// <summary>
        /// Devuelve un usuario por su ID.
        /// 
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUser(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("User not found.");

            var userDto = new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = user.Roles.Select(r => r.Role).ToList()
            };

            return Ok(userDto);
        }


        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register([FromBody] UserRegisterDto userDto)
        {

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = userDto.Email,
                CreatedAt = DateTime.UtcNow
            };

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.PasswordHash = hashedPassword;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            var createdUserDto = new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = new List<string>() // aún no hemos asignado roles
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, createdUserDto);
        }

        // PUT: api/users/{id}
        /// <summary>
        /// Actualiza la información de un usuario.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto updatedDto)
        {
            if (id != updatedDto.Id)
                return BadRequest("ID mismatch.");

            var existingUser = await _context.Users
                                             .Include(u => u.Roles)
                                             .FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
                return NotFound("User not found.");

            existingUser.Email = updatedDto.Email;


            if (!string.IsNullOrWhiteSpace(updatedDto.Password))
            {
                bool samePassword = BCrypt.Net.BCrypt.Verify(updatedDto.Password, existingUser.PasswordHash);
                if (!samePassword)
                {
                    existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedDto.Password);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/users/{id}
        /// <summary>
        /// Elimina un usuario del sistema.
        /// </summary>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
