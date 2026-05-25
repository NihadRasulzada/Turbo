using System;
using System.Collections.Generic;
using System.Text;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.Login;

public sealed record LoginResponse(string AccessToken, string RefreshToken, Guid UserId);