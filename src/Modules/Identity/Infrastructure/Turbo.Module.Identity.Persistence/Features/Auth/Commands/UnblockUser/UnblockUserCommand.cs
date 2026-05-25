using MediatR;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;

public sealed record UnblockUserCommand(Guid UserId) : IRequest;
