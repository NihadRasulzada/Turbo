using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Login;

public sealed record LoginRequest(
    string Email,
    string Password
) : ICommand<AppConc.Response<LoginResponse>>;
