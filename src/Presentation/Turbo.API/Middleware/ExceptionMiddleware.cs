using System.Net;
using System.Text.Json;
using FluentValidation;
using Turbo.Module.Identity.Domain.Exceptions;

namespace Turbo.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await WriteJson(context, new
            {
                errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = ex switch
            {
                UserNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidCredentialsException => (int)HttpStatusCode.Unauthorized,
                EmailAlreadyExistsException => (int)HttpStatusCode.Conflict,
                InvalidRefreshTokenException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.BadRequest
            };
            await WriteJson(context, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await WriteJson(context, new { error = "Internal server error." });
        }
    }

    private static Task WriteJson(HttpContext ctx, object obj)
    {
        ctx.Response.ContentType = "application/json";
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(obj));
    }
}