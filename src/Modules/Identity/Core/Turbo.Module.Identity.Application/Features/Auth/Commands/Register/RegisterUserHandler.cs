using MediatR;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Exceptions;

namespace Turbo.Module.Identity.Application.Features.Auth.Commands.Register;

public class RegisterUserHandler(
    IWriteDbContext writeDb,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await writeDb.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (exists)
            throw new EmailAlreadyExistsException(request.Email);

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

        writeDb.Users.Add(user);
        await writeDb.SaveChangesAsync(cancellationToken);

        await eventPublisher.PublishAsync(new UserRegisteredEvent(
            user.Id, user.Email, user.FirstName, user.LastName, user.CreatedAt
        ), cancellationToken);

        return new RegisterUserResult(user.Id, user.Email);
    }
}
