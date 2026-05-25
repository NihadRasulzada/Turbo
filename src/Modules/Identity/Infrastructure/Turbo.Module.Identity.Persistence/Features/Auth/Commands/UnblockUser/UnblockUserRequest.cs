using Turbo.Shared.Application.Abstraction;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;

public sealed record UnblockUserRequest(Guid UserId) : ICommand<AppConc.Response>;
