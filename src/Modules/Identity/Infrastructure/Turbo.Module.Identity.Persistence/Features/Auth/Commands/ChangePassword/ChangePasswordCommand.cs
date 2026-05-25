using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest;