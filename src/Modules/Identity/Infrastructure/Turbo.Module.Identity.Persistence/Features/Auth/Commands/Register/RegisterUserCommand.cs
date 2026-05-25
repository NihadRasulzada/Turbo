using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<RegisterUserResponse>;