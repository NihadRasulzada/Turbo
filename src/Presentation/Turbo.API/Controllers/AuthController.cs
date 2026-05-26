using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Turbo.API.Extensions;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.Login;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.RefreshToken;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Web.Controllers;
using AppConc = Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.API.Controllers;

/// <summary>
/// İstifadəçi autentifikasiyası ilə bağlı əməliyyatları idarə edir.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(ICommandDispatcher commandDispatcher) : ControllerBase
{
    /// <summary>
    /// Yeni istifadəçi qeydiyyatı həyata keçirir.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(CreatedResponse<RegisterUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<RegisterUserRequest, AppConc.Response<RegisterUserResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// İstifadəçi girişi həyata keçirir və token qaytarır.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<LoginRequest, AppConc.Response<LoginResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Köhnə refresh token ilə yeni access və refresh token alır.
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(SuccessResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<RefreshTokenRequest, AppConc.Response<RefreshTokenResponse>>(request, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Autentifikasiya olunmuş istifadəçinin şifrəsini dəyişir.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordHttpBody body, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new ChangePasswordRequest(
            userId,
            body.CurrentPassword,
            body.NewPassword,
            body.ConfirmPassword);
        var result = await commandDispatcher
            .DispatchAsync<ChangePasswordRequest, AppConc.Response>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Müəyyən istifadəçini bloklayır.
    /// </summary>
    [HttpPost("users/{userId:guid}/block")]
    [Authorize]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUser(
        [FromRoute] Guid userId,
        [FromBody] BlockUserHttpBody body,
        CancellationToken ct)
    {
        var command = new BlockUserRequest(userId, body.DurationSeconds);
        var result = await commandDispatcher
            .DispatchAsync<BlockUserRequest, AppConc.Response>(command, ct);
        return this.HandleServiceResponse(result);
    }

    /// <summary>
    /// Bloklanmış istifadəçini blokdan çıxarır.
    /// </summary>
    [HttpPost("users/{userId:guid}/unblock")]
    [Authorize]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnblockUser(
        [FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await commandDispatcher
            .DispatchAsync<UnblockUserRequest, AppConc.Response>(
                new UnblockUserRequest(userId), ct);
        return this.HandleServiceResponse(result);
    }
}

/// <summary>Şifrə dəyişmə sorğusunun HTTP body-si.</summary>
public sealed record ChangePasswordHttpBody(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

/// <summary>İstifadəçi bloklama sorğusunun HTTP body-si.</summary>
public sealed record BlockUserHttpBody(int DurationSeconds);
