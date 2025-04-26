using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Users;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exceptions;
using Simpled.Dtos.Teams;

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
                Name = u.Name,
                Email = u.Email,
                ImageUrl = u.ImageUrl,
                achievementsCompleted = u.Achievements.Count,
                CreatedAt = u.CreatedAt,
                Roles = u.Roles.Select(r => r.Role).ToList()
            });
        }

        public async Task<UserReadDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Achievements)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            Console.WriteLine(user.Achievements);
            return new UserReadDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                achievementsCompleted = user.Achievements.Count,
                Teams = new List<TeamDto>
    {
                    new TeamDto { Key = "1" ,Name = "Equipo Alpha", Role = "Admin" },
                    new TeamDto { Key = "2" ,Name = "Equipo Beta", Role = "Miembro" }
                },
                CreatedAt = user.CreatedAt,
                Roles = user.Roles.Select(r => r.Role).ToList()
            };
        }

        public async Task<UserReadDto> RegisterAsync(UserRegisterDto userDto, IFormFile? image)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Roles = new List<UserRole>(),
                ImageUrl = "/images/default/avatar-default.jpg"
            };

            user.Roles.Add(new UserRole
            {
                UserId = user.Id,
                Role = "viewer"
            });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (image != null)
            {
                string uploadsFolder = Path.Combine("wwwroot", "images", user.Id.ToString());

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Console.WriteLine($"Carpeta creada: {uploadsFolder}");
                }

                string filePath = Path.Combine(uploadsFolder, "image.jpg");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                user.ImageUrl = $"/images/{user.Id.ToString()}/image.jpg";
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return new UserReadDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = user.Roles.Select(r => r.Role).ToList(),
                ImageUrl = user.ImageUrl,
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
