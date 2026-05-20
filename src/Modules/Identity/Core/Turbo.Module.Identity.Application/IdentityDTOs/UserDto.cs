namespace Turbo.Module.Identity.Application.IdentityDTOs;

public record LoginRequestDto(
    string Username,
    string Password
);

public record RefreshTokenRequestDto(
    string Username,
    string RefreshToken
);

public record AuthResponseDto(
    string Token,
    string RefreshToken,
    DateTime TokenExpiresAt
);

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

public record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Username,
    string Email,
    string Password,
    string ConfirmPassword
);
