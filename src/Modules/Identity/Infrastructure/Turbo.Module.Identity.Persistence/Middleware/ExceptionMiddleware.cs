using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Turbo.Module.Identity.Application.Common.Models;
using Turbo.Module.Identity.Domain.Exceptions;

namespace Turbo.Module.Identity.Persistence.Middleware;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException ex => (
                HttpStatusCode.BadRequest,
                new ErrorResponse(
                    "VALIDATION_ERROR",
                    string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
                )
            ),
            UserNotFoundException ex => (
                HttpStatusCode.NotFound,
                new ErrorResponse("USER_NOT_FOUND", ex.Message)
            ),
            EmailAlreadyExistsException ex => (
                HttpStatusCode.Conflict,
                new ErrorResponse("EMAIL_ALREADY_EXISTS", ex.Message)
            ),
            InvalidCredentialsException ex => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse("INVALID_CREDENTIALS", ex.Message)
            ),
            InvalidRefreshTokenException ex => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse("INVALID_REFRESH_TOKEN", ex.Message)
            ),
            UserBlockedException ex => (
                HttpStatusCode.Forbidden,
                new ErrorResponse("USER_BLOCKED", ex.Message)
            ),
            PasswordMismatchException ex => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("PASSWORD_MISMATCH", ex.Message)
            ),
            DomainException ex => (
                HttpStatusCode.BadRequest,
                new ErrorResponse("DOMAIN_ERROR", ex.Message)
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse("INTERNAL_SERVER_ERROR", "An unexpected error occurred.")
            )
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}
