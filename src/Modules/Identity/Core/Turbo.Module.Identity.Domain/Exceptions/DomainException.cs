namespace Turbo.Module.Identity.Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public class UserNotFoundException(Guid userId)
    : DomainException($"User '{userId}' not found.");

public class InvalidCredentialsException()
    : DomainException("Email or password is incorrect.");

public class EmailAlreadyExistsException(string email)
    : DomainException($"Email '{email}' is already registered.");

public class InvalidRefreshTokenException()
    : DomainException("Refresh token is invalid or expired.");

public class UserBlockedException()
    : DomainException("User is blocked. Please contact support.");

public class PasswordMismatchException()
    : DomainException("Passwords do not match.");
