using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Turbo.Module.Identity.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest;
