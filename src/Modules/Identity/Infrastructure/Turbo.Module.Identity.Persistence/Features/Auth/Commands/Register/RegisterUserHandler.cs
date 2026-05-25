using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Events;
using Turbo.Module.Identity.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed class RegisterUserHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb,
    IPasswordHasher passwordHasher,
    IEventPublisher eventPublisher
) : ICommandHandler<RegisterUserRequest, AppConc.Response<RegisterUserResponse>>
{
    public async Task<AppConc.Response<RegisterUserResponse>> HandleAsync(
        RegisterUserRequest command, CancellationToken ct = default)
    {
        var normalizedEmail = command.Email.ToUpperInvariant();

        var exists = await readDb.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail, ct);

        if (exists)
            return AppConc.Response<RegisterUserResponse>.Conflict(
                $"Email '{command.Email}' is already registered.");

        var passwordHash = passwordHasher.Hash(command.Password);
        var user = User.Create(command.Email, passwordHash, command.FirstName, command.LastName);

        writeDb.Add(user);
        await writeDb.SaveChangesAsync(ct);

        await eventPublisher.PublishAsync(
            new UserRegisteredEvent(user.Id, user.Email, user.FirstName, user.LastName),
            ct);

        return AppConc.Response<RegisterUserResponse>.Created(
            new RegisterUserResponse(user.Id, user.Email));
    }
}
