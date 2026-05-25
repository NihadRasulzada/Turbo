using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb,
    IPasswordHasher passwordHasher
) : ICommandHandler<ChangePasswordRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        ChangePasswordRequest command, CancellationToken ct = default)
    {
        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
            return AppConc.Response.NotFound($"User '{command.UserId}' not found.");

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            return AppConc.Response.Unauthorized("Current password is incorrect.");

        writeDb.Attach(user);
        var newHash = passwordHasher.Hash(command.NewPassword);
        user.ChangePassword(newHash);
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response.Success("Password changed successfully.");
    }
}
