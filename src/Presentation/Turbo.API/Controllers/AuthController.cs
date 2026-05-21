using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Turbo.Module.Identity.Application.Features.Auth.Commands.ChangePassword;
using Turbo.Module.Identity.Application.Features.Auth.Commands.Register;
using Turbo.Module.Identity.Application.Features.Auth.Queries.Login;
using Turbo.Module.Identity.Application.Features.Auth.Queries.RefreshTokenQuery;

namespace Turbo.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Register), new { result.UserId }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword);
        await mediator.Send(command, ct);
        return NoContent();
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
