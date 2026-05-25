using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Persistence.Context;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;


public sealed class ChangePasswordHandler(
    IdentityQueryContext queryDb,
    IdentityCommandContext commandDb,
    IPasswordHasher passwordHasher
) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await queryDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new InvalidCredentialsException();

        var trackedUser = await commandDb.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        var newHash = passwordHasher.Hash(request.NewPassword);
        trackedUser.ChangePassword(newHash);

        await commandDb.SaveChangesAsync(cancellationToken);
    }
}
