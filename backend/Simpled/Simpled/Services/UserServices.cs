using Microsoft.EntityFrameworkCore;
using Simpled.Data;
using Simpled.Dtos.Users;
using Simpled.Models;
using Simpled.Repository;
using Simpled.Exception;
using Simpled.Dtos.Teams;
using Simpled.Dtos.Teams.TeamMembers;


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
                AchievementsCompleted = u.Achievements.Count,
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

            var memberships = await _context.TeamMembers
                .Include(tm => tm.Team)
                    .ThenInclude(t => t.Owner)
                .Include(tm => tm.Team)
                    .ThenInclude(t => t.Members)
                        .ThenInclude(m => m.User)
                .Where(tm => tm.UserId == id)
                .ToListAsync();

            var teamDtos = memberships.Select(tm => new TeamReadDto
            {
                Id = tm.TeamId,
                Name = tm.Team!.Name,
                OwnerId = tm.Team.OwnerId,
                OwnerName = tm.Team.Owner!.Name,
                Role = tm.Role,  
                Members = tm.Team.Members.Select(m => new TeamMemberDto
                {
                    UserId = m.UserId,
                    UserName = m.User!.Name,
                    Role = m.Role
                })
            });

            return new UserReadDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                AchievementsCompleted = user.Achievements.Count,
                CreatedAt = user.CreatedAt,
                Roles = user.Roles.Select(r => r.Role).ToList(),
                Teams = teamDtos.ToList()
            };
        }


        public async Task<UserReadDto> RegisterAsync(UserRegisterDto userDto, IFormFile ?image)
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



        public async Task<bool> UpdateAsync(UserUpdateDto updatedDto, IFormFile? image)
        {
            var existing = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == updatedDto.Id);
            if (existing == null)
                throw new NotFoundException("Usuario no encontrado.");

            existing.Email = updatedDto.Email;
            existing.Name = updatedDto.Name;

            if (!string.IsNullOrWhiteSpace(updatedDto.Password))
            {
                bool samePassword = BCrypt.Net.BCrypt.Verify(updatedDto.Password, existing.PasswordHash);
                if (!samePassword)
                {
                    existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedDto.Password);
                }
            }

            if (image != null)
            {
                string uploadsFolder = Path.Combine("wwwroot", "images", existing.Id.ToString());

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

                existing.ImageUrl = $"/images/{existing.Id.ToString()}/image.jpg";
                _context.Users.Update(existing);
                await _context.SaveChangesAsync();
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
