using MediatR;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;

public sealed record BlockUserCommand(
    Guid UserId,
    int DurationSeconds
) : IRequest;