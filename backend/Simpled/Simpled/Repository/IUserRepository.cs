using Simpled.Dtos.Users;

namespace Simpled.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(Guid id);
        Task<UserReadDto> RegisterAsync(UserRegisterDto userDto, IFormFile ?image);
        Task<bool> UpdateAsync(UserUpdateDto userDto, IFormFile? image);
        Task<bool> DeleteAsync(Guid id);
    }
}
