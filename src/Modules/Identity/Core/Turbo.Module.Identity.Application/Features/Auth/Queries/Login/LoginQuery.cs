using MediatR;

namespace Turbo.Module.Identity.Application.Features.Auth.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(string AccessToken, string RefreshToken, Guid UserId);
