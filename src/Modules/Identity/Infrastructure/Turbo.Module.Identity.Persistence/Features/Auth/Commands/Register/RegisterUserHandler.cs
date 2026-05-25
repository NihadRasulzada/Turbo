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

        // İlk yoxlama — replica-da oxuyur; concurrent race üçün aşağıda DB constraint var
        var exists = await readDb.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail, ct);

        if (exists)
            return AppConc.Response<RegisterUserResponse>.Conflict(
                $"Email '{command.Email}' is already registered.");

        var user = User.Create(
            command.Email,
            passwordHasher.Hash(command.Password),
            command.FirstName,
            command.LastName);

        writeDb.Add(user);

        try
        {
            await writeDb.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException?.Message.Contains("23505", StringComparison.Ordinal) == true
               || ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true)
        {
            // Concurrent qeydiyyat: read replica-da görünməmiş, amma DB unique index tutan
            return AppConc.Response<RegisterUserResponse>.Conflict(
                $"Email '{command.Email}' is already registered.");
        }

        // DB commit uğurlu olduqdan sonra hadisəni yayımla
        await eventPublisher.PublishAsync(
            new UserRegisteredEvent(user.Id, user.Email, user.FirstName, user.LastName),
            ct);

        return AppConc.Response<RegisterUserResponse>.Created(
            new RegisterUserResponse(user.Id, user.Email));
    }
}
