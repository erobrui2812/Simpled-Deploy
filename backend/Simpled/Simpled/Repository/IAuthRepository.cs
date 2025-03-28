using Simpled.Dtos.Auth;
using System.Threading.Tasks;

namespace Simpled.Repository
{
    public interface IAuthRepository
    {
        Task<string?> LoginAsync(LoginRequestDto loginDto);
    }
}
