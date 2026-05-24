using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Turbo.Module.Identity.Application.Common.Interfaces;
using Turbo.Module.Identity.Application.DTOs;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Infrastructure.Services;

namespace Turbo.Module.Identity.Persistence.Services;

public class UserService(
    UserManager<AppUser> userManager,
    TokenService tokenService
) : IUserService
{
    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");

        var user = AppUser.Create(request.Email, request.FirstName, request.LastName);

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(
        RefreshTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedAccessException("Invalid access token.");

        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Invalid token claims.");

        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("User not found.");

        var hashedIncoming = tokenService.HashToken(request.RefreshToken);

        if (!user.HasValidRefreshToken(hashedIncoming))
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new KeyNotFoundException($"User '{userId}' not found.");

        var result = await userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Password change failed: {errors}");
        }

        user.RevokeRefreshToken();
        await userManager.UpdateAsync(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(AppUser user)
    {
        var (accessToken, accessTokenExpiresAt) = tokenService.GenerateAccessToken(user);
        var (rawRefreshToken, hashedRefreshToken, refreshTokenExpiresAt) =
            tokenService.GenerateRefreshToken();

        user.SetRefreshToken(hashedRefreshToken, refreshTokenExpiresAt);
        await userManager.UpdateAsync(user);

        return new AuthResponseDto(
            AccessToken: accessToken,
            RefreshToken: rawRefreshToken,
            AccessTokenExpiresAt: accessTokenExpiresAt,
            UserId: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName
        );
    }
}
