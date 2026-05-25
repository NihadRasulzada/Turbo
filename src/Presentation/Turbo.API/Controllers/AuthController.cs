using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Turbo.Module.Identity.Application.Common.Models;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.BlockUser;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.ChangePassword;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.Register;
using Turbo.Module.Identity.Persistence.Features.Auth.Commands.UnblockUser;
using Turbo.Module.Identity.Persistence.Features.Auth.Queries.Login;
using Turbo.Module.Identity.Persistence.Features.Auth.Queries.RefreshToken;

namespace Turbo.API.Controllers;

/// <summary>
/// İstifadəçi autentifikasiyası ilə bağlı əməliyyatları idarə edir.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Yeni istifadəçi qeydiyyatı həyata keçirir.
    /// </summary>
    /// <param name="command">Qeydiyyat məlumatları.</param>
    /// <param name="ct">Ləğvetmə tokeni.</param>
    /// <returns>Yeni istifadəçinin ID-si və email-i.</returns>
    /// <response code="201">İstifadəçi uğurla qeydiyyatdan keçdi.</response>
    /// <response code="400">Məlumatlar yanlışdır və ya validasiya xətası baş verdi.</response>
    /// <response code="409">Bu email artıq qeydiyyatdan keçib.</response>
    /// <response code="500">Server xətası baş verdi.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand command,
        CancellationToken ct)
    {
        var result = await userService.RegisterAsync(request, ct);
        return CreatedAtAction(nameof(Register), new { result.UserId }, result);
    }

    /// <summary>
    /// İstifadəçi girişi həyata keçirir və token qaytarır.
    /// </summary>
    /// <param name="query">Email və şifrə məlumatları.</param>
    /// <param name="ct">Ləğvetmə tokeni.</param>
    /// <returns>Access token, refresh token və istifadəçi ID-si.</returns>
    /// <response code="200">Giriş uğurlu oldu.</response>
    /// <response code="400">Məlumatlar yanlışdır və ya validasiya xətası baş verdi.</response>
    /// <response code="401">Email və ya şifrə yanlışdır.</response>
    /// <response code="403">İstifadəçi bloklanıb.</response>
    /// <response code="500">Server xətası baş verdi.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(
        [FromBody] LoginQuery query,
        CancellationToken ct)
    {
        var result = await userService.LoginAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Köhnə refresh token ilə yeni access və refresh token alır.
    /// </summary>
    /// <param name="query">Mövcud refresh token.</param>
    /// <param name="ct">Ləğvetmə tokeni.</param>
    /// <returns>Yeni access token və refresh token.</returns>
    /// <response code="200">Token uğurla yeniləndi.</response>
    /// <response code="400">Məlumatlar yanlışdır və ya validasiya xətası baş verdi.</response>
    /// <response code="401">Refresh token etibarsızdır və ya müddəti bitib.</response>
    /// <response code="500">Server xətası baş verdi.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenQuery query,
        CancellationToken ct)
    {
        var result = await userService.RefreshTokenAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Autentifikasiya olunmuş istifadəçinin şifrəsini dəyişir.
    /// </summary>
    /// <param name="request">Cari və yeni şifrə məlumatları.</param>
    /// <param name="ct">Ləğvetmə tokeni.</param>
    /// <returns>Məzmun yoxdur.</returns>
    /// <response code="204">Şifrə uğurla dəyişdirildi.</response>
    /// <response code="400">Məlumatlar yanlışdır, validasiya xətası və ya şifrələr uyğun gəlmir.</response>
    /// <response code="401">İstifadəçi autentifikasiya olunmayıb və ya cari şifrə yanlışdır.</response>
    /// <response code="404">İstifadəçi tapılmadı.</response>
    /// <response code="500">Server xətası baş verdi.</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmPassword);
        await mediator.Send(command, ct);
        return NoContent();
    }

    /// <summary>
    /// Adminin müəyyən istifadəçini bloklamasını həyata keçirir.
    /// </summary>
    /// <param name="userId">Bloklanacaq istifadəçinin ID-si.</param>
    /// <param name="request">Bloklama müddəti (saniyə ilə).</param>
    /// <param name="ct">Ləğvetmə tokeni.</param>
    /// <returns>Məzmun yoxdur.</returns>
    /// <response code="204">İstifadəçi uğurla bloklandı.</response>
    /// <response code="400">Məlumatlar yanlışdır və ya validasiya xətası baş verdi.</response>
    /// <response code="401">İcazəsiz giriş.</response>
    /// <response code="404">İstifadəçi tapılmadı.</response>
    /// <response code="500">Server xətası baş verdi.</response>
    [HttpPost("users/{userId:guid}/block")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BlockUser(
        [FromRoute] Guid userId,
        [FromBody] BlockUserRequest request,
        CancellationToken ct)
    {
        var command = new BlockUserCommand(userId, request.DurationSeconds);
        await mediator.Send(command, ct);
        return NoContent();
    }

    /// <summary>
    /// Adminin bloklanmış istifadəçini blokdan çıxarmasını həyata keçirir.
    /// </summary>
    /// <param name="userId">Blokdan çıxarılacaq istifadəçinin ID-si.</param>
    /// <param name="ct">Ləğvetmə tokeni.</param>
    /// <returns>Məzmun yoxdur.</returns>
    /// <response code="204">İstifadəçi uğurla blokdan çıxarıldı.</response>
    /// <response code="401">İcazəsiz giriş.</response>
    /// <response code="404">İstifadəçi tapılmadı.</response>
    /// <response code="500">Server xətası baş verdi.</response>
    [HttpPost("users/{userId:guid}/unblock")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UnblockUser(
        [FromRoute] Guid userId,
        CancellationToken ct)
    {
        var command = new UnblockUserCommand(userId);
        await mediator.Send(command, ct);
        return NoContent();
    }
}

/// <summary>Şifrə dəyişmə sorğusu.</summary>
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

/// <summary>İstifadəçi bloklama sorğusu.</summary>
public sealed record BlockUserRequest(int DurationSeconds);

