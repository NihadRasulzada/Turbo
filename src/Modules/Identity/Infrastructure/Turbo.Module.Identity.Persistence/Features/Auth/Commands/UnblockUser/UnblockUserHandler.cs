using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Persistence.Contexts;
using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;

public sealed class UnblockUserHandler(
    IIdentityWriteDbContext writeDb,
    IIdentityReadDbContext readDb
) : ICommandHandler<UnblockUserRequest, AppConc.Response>
{
    public async Task<AppConc.Response> HandleAsync(
        UnblockUserRequest command, CancellationToken ct = default)
    {
        var user = await readDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, ct);

        if (user is null)
            return AppConc.Response.NotFound($"User '{command.UserId}' not found.");

        writeDb.Attach(user);
        user.Unblock();
        await writeDb.SaveChangesAsync(ct);

        return AppConc.Response.Success("User unblocked successfully.");
    }
}
