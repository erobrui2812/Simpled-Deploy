using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Users;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;

namespace Simpled.Services
{
    public class UserService : IUserRepository
    {
        private readonly SimpledDbContext _context;

        public UserService(SimpledDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.Include(u => u.Roles).ToListAsync();
            return users.Select(u => new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                Roles = u.Roles.Select(r => r.Role).ToList()
            });
        }

        public async Task<UserReadDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            return new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = user.Roles.Select(r => r.Role).ToList()
            };
        }

        public async Task<UserReadDto> RegisterAsync(UserRegisterDto userDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = userDto.Email,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = new List<string>()
            };
        }

        public async Task<bool> UpdateAsync(UserUpdateDto updatedDto)
        {
            var existing = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == updatedDto.Id);
            if (existing == null)
                throw new NotFoundException("Usuario no encontrado.");

            existing.Email = updatedDto.Email;

            if (!string.IsNullOrWhiteSpace(updatedDto.Password))
            {
                bool samePassword = BCrypt.Net.BCrypt.Verify(updatedDto.Password, existing.PasswordHash);
                if (!samePassword)
                {
                    existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedDto.Password);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
