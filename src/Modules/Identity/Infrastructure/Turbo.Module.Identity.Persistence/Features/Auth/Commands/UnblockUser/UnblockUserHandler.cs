using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Persistence.Context;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;

public sealed class UnblockUserHandler(
    IdentityCommandContext commandDb
) : IRequestHandler<UnblockUserCommand>
{
    public async Task Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await commandDb.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        user.Unblock();

        await commandDb.SaveChangesAsync(cancellationToken);
    }
}