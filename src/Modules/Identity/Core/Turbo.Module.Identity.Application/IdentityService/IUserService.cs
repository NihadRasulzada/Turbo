using Turbo.Module.Identity.Application.IdentityDTOs;
using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Application.IdentityService;

public interface IUserService
{
    Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task<Response> ChangePasswordAsync(string userId, ChangePasswordRequestDto dto);
    Task<Response> RegisterAsync(RegisterRequestDto dto);
}
