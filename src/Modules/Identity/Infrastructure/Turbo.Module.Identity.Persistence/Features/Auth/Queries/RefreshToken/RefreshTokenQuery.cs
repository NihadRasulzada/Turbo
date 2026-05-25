using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.RefreshToken;

public sealed record RefreshTokenQuery(string RefreshToken) : IRequest<RefreshTokenResponse>;