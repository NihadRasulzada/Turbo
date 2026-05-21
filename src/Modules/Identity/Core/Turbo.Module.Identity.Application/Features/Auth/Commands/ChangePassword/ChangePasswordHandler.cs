using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Exceptions;

namespace Turbo.Module.Identity.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordHandler(
    IWriteDbContext writeDb,
    IPasswordHasher passwordHasher
) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await writeDb.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new InvalidCredentialsException();

        var newHash = passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(newHash);

        await writeDb.SaveChangesAsync(cancellationToken);
    }
}
