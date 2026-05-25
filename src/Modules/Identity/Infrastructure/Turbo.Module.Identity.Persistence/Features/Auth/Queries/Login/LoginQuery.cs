using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Queries.Login;

public sealed record LoginQuery(string Email, string Password) : IRequest<LoginResponse>;