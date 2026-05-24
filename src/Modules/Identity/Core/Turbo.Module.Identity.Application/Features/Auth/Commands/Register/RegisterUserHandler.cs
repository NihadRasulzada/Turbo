using MediatR;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Turbo.Module.Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Turbo.Module.Identity.Application.Features.Auth.Commands.Register;

public class RegisterUserHandler(
    UserManager<AppUser> userManager,
    IEventPublisher eventPublisher
) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByEmailAsync(request.Email);

        if (exists is not null)
            throw new EmailAlreadyExistsException(request.Email);

        var user = AppUser.Create(request.Email, request.FirstName, request.LastName);

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        await eventPublisher.PublishAsync(new UserRegisteredEvent(
            user.Id, user.Email!, user.FirstName, user.LastName, DateTime.UtcNow
        ), cancellationToken);

        return new RegisterUserResult(user.Id, user.Email!);
    }
}
