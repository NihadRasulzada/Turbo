using MediatR;

namespace Turbo.Module.Identity.Application.Features.Auth.Queries.RefreshTokenQuery;

public record RefreshTokenQuery(string RefreshToken) : IRequest<RefreshTokenResult>;

public record RefreshTokenResult(string AccessToken, string NewRefreshToken);
