using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Persistence.Context;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;

public sealed class BlockUserHandler(
    IdentityCommandContext commandDb
) : IRequestHandler<BlockUserCommand>
{
    public async Task Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await commandDb.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        user.Block(request.DurationSeconds);

        await commandDb.SaveChangesAsync(cancellationToken);
    }
}
