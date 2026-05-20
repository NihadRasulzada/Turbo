using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Turbo.Module.Identity.Application.IdentityDTOs;
using Turbo.Module.Identity.Application.IdentityService;
using Turbo.Module.Identity.Domain.Entity;
using Turbo.Module.Identity.Infrastructure.Services;
using Turbo.Shared.Application.ResponseObject;
using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Module.Identity.Persistence.IdentityService;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;

    public UserService(
        UserManager<AppUser> userManager,
        TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);

        if (user == null)
            return Response<AuthResponseDto>.Unauthorized("Username or password is wrong");

        if (await _userManager.IsLockedOutAsync(user))
            return Response<AuthResponseDto>.Unauthorized("Too many failed attempts. Please try again later.");

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
        {
            await _userManager.AccessFailedAsync(user);

            if (await _userManager.IsLockedOutAsync(user))
                return Response<AuthResponseDto>.Unauthorized("Too many failed attempts. Please try again later.");

            return Response<AuthResponseDto>.Unauthorized("Username or password is wrong");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var (token, expiresAt) = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

        user.RefreshToken = refreshTokenHash;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return Response<AuthResponseDto>.Success(new AuthResponseDto(token, refreshToken, expiresAt), "Login successful");
    }

    public async Task<Response<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        var refreshTokenHash = _tokenService.HashRefreshToken(dto.RefreshToken);
        var refreshTokenMatches =
            user?.RefreshToken == refreshTokenHash ||
            user?.RefreshToken == dto.RefreshToken;

        if (user == null ||
            !refreshTokenMatches ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Response<AuthResponseDto>.Unauthorized("Invalid refresh token");
        }

        var (newToken, newExpiresAt) = _tokenService.CreateToken(user);
        var newRefreshToken = _tokenService.CreateRefreshToken();
        var newRefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);

        user.RefreshToken = newRefreshTokenHash;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return Response<AuthResponseDto>.Success(new AuthResponseDto(newToken, newRefreshToken, newExpiresAt), "Token refreshed");
    }

    public async Task<Response> ChangePasswordAsync(string userId, ChangePasswordRequestDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Response.NotFound("User not found");

        if (dto.NewPassword != dto.ConfirmPassword)
            return Response.ValidationError(new[]
            {
            new CustomValidationError("ConfirmPassword", "Passwords do not match")
        });

        var checkOldPassword = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);

        if (!checkOldPassword)
            return Response.BadRequest("Current password is incorrect");

        var result = await _userManager.ChangePasswordAsync(
            user,
            dto.CurrentPassword,
            dto.NewPassword
        );

        if (!result.Succeeded)
        {
            return Response.ValidationError(
                result.Errors.Select(e =>
                    new CustomValidationError("NewPassword", e.Description)
                )
            );
        }

        return Response.Success("Password changed successfully");
    }

    public async Task<Response> RegisterAsync(RegisterRequestDto dto)
    {
        
        var existingByUsername = await _userManager.FindByNameAsync(dto.Username);
        if (existingByUsername != null)
            return Response.Conflict("Username is already taken");

        
        var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingByEmail != null)
            return Response.Conflict("Email is already registered");

        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return Response.ValidationError(
                result.Errors.Select(e =>
                    new CustomValidationError("Password", e.Description)
                )
            );
        }

        return Response.Success("Registration successful");
    }
}
