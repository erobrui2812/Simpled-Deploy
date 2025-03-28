using Simpled.Dtos.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simpled.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(Guid id);
        Task<UserReadDto> RegisterAsync(UserRegisterDto userDto);
        Task<bool> UpdateAsync(UserUpdateDto userDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
