using System.Reflection;
using FluentValidation;
using Turbo.Shared.Application.Abstraction;
using Turbo.Shared.Application.Pipeline;
using Turbo.Shared.Application.ResponseObject;
using Turbo.Shared.Application.ResponseObject.Concreate;
using Turbo.Shared.Application.ResponseObject.Enums;

namespace Turbo.Shared.Infrastructure.Pipeline;

public sealed class ValidationPipelineBehavior<TCommand, TResponse>(
    IEnumerable<IValidator<TCommand>> validators)
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : Response
{
    public async Task<TResponse> HandleAsync(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(command, ct)));

        var errors = results
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => new CustomValidationError(e.PropertyName, e.ErrorMessage))
            .ToList();

        if (errors.Count > 0)
            return CreateValidationError(errors);

        return await next();
    }

    /// <summary>
    /// Creates a validation-error TResponse without knowing the concrete type at compile time.
    /// Both <see cref="Response"/> and <see cref="Response{T}"/> expose a static
    /// <c>ValidationError(IEnumerable&lt;CustomValidationError&gt;)</c> factory — we call it
    /// via a one-time cached MethodInfo lookup.
    /// </summary>
    private static TResponse CreateValidationError(IEnumerable<CustomValidationError> errors)
    {
        var type = typeof(TResponse);

        if (type == typeof(Response))
            return (TResponse)(object)Response.ValidationError(errors);

        var method = type.GetMethod(
            nameof(Response.ValidationError),
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
            [typeof(IEnumerable<CustomValidationError>)]);

        // Fallback: if TResponse is some subclass that doesn't declare its own factory,
        // construct a Response with the right status and cast.
        if (method is null)
            return (TResponse)(object)new Response(ResponseStatusCode.ValidationError)
            {
                ValidationErrors = errors
            };

        return (TResponse)method.Invoke(null, [errors])!;
    }
}
