using MediatR;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Events;
using Turbo.Module.Identity.Domain.Exceptions;
using Turbo.Module.Identity.Persistence.Context;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed class RegisterUserHandler(
    IdentityCommandContext commandDb,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();

        var exists = await commandDb.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        if (exists)
            throw new EmailAlreadyExistsException(request.Email);

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

        commandDb.Users.Add(user);
        await commandDb.SaveChangesAsync(cancellationToken);

        await eventPublisher.PublishAsync(
            new UserRegisteredEvent(user.Id, user.Email, user.FirstName, user.LastName),
            cancellationToken);

        return new RegisterUserResponse(user.Id, user.Email);
    }
}