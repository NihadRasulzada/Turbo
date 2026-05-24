using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Application.DTOs;
using Turbo.Module.Identity.Application.Features.Auth.Commands.ChangePassword;
using Turbo.Module.Identity.Application.Features.Auth.Commands.Register;
using Turbo.Module.Identity.Application.Features.Auth.Queries.Login;
using Turbo.Module.Identity.Application.Features.Auth.Queries.RefreshTokenQuery;

namespace Turbo.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        var result = await userService.RegisterAsync(request, ct);
        return CreatedAtAction(nameof(Register), new { result.UserId }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var result = await userService.LoginAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        var result = await userService.RefreshTokenAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await userService.ChangePasswordAsync(userId, request, ct);
        return NoContent();
    }
}
