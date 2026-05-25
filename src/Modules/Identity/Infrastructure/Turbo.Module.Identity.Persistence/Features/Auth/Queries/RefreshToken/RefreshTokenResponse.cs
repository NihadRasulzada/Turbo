using System;
using System.Collections.Generic;
using System.Text;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.RefreshToken;

public sealed record RefreshTokenResponse(string AccessToken, string NewRefreshToken);