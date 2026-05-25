using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;

public sealed record BlockUserRequest(
    Guid UserId,
    int DurationSeconds
) : ICommand<AppConc.Response>;
