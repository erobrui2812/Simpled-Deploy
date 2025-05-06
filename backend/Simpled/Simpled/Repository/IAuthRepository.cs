using Simpled.Dtos.Auth;

namespace Simpled.Repository
{
    public interface IAuthRepository
    {
        Task<LoginResultDto?> LoginAsync(LoginRequestDto loginDto);
        Task<LoginResultDto?> ExternalLoginAsync(ExternalLoginDto dto);
    }
}
