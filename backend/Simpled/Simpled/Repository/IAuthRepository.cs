using Simpled.Dtos.Auth;
using System.Threading.Tasks;

namespace Simpled.Repository
{
    public interface IAuthRepository
    {
        Task<LoginResultDto?> LoginAsync(LoginRequestDto loginDto);
    }
}
