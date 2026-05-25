using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenRequest(
    string RefreshToken
) : ICommand<AppConc.Response<RefreshTokenResponse>>;
