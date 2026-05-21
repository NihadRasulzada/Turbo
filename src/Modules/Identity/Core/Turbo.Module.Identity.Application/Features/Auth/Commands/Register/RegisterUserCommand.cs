using MediatR;

namespace Turbo.Module.Identity.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<RegisterUserResult>;

public record RegisterUserResult(Guid UserId, string Email);
