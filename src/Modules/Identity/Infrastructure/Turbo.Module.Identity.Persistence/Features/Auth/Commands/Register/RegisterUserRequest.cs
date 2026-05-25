using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : ICommand<AppConc.Response<RegisterUserResponse>>;
