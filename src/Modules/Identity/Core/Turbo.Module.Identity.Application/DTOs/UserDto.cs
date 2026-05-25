using System;
using System.Collections.Generic;
using System.Text;

namespace Turbo.Module.Identity.Application.DTOs;

public record LoginRequestDto(
    string Email,
    string Password
);

public record RegisterRequestDto(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
);

public record RefreshTokenRequestDto(
    string AccessToken,
    string RefreshToken
);

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
);