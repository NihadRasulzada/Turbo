using Turbo.Shared.Application.ResponseObject.Abstraction;
using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Shared.Application.ResponseObject.Extensions;

public static class ResponseExtensions
{
    /// <summary>
    /// Executes an action if the response is successful
    /// </summary>
    public static IResponse<T> OnSuccess<T>(this IResponse<T> response, Action<T> action)
    {
        if (response.IsSuccess && response.Data != null)
        {
            action(response.Data);
        }
        return response;
    }

    /// <summary>
    /// Executes an action if the response is failure
    /// </summary>
    public static IResponse<T> OnFailure<T>(this IResponse<T> response, Action<IResponse<T>> action)
    {
        if (response.IsFailure)
        {
            action(response);
        }
        return response;
    }

    /// <summary>
    /// Maps the data from one type to another
    /// </summary>
    public static Response<TResult> Map<T, TResult>(
        this IResponse<T> response,
        Func<T, TResult> mapper
    )
    {
        if (response.IsSuccess && response.Data != null)
        {
            return Response<TResult>.Success(mapper(response.Data), response.Message);
        }

        return new Response<TResult>((Response)response, default(TResult));
    }

    /// <summary>
    /// Checks if response has validation errors
    /// </summary>
    public static bool HasValidationErrors(this IResponse response)
    {
        return response.ValidationErrors != null && response.ValidationErrors.Any();
    }

    /// <summary>
    /// Checks if response has errors
    /// </summary>
    public static bool HasErrors(this IResponse response)
    {
        return response.Errors != null && response.Errors.Any();
    }

    /// <summary>
    /// Gets first validation error message if exists
    /// </summary>
    public static string GetFirstValidationError(this IResponse response)
    {
        return response.ValidationErrors?.FirstOrDefault()?.ErrorMessage;
    }

    /// <summary>
    /// Gets first error description if exists
    /// </summary>
    public static string GetFirstError(this IResponse response)
    {
        return response.Errors?.FirstOrDefault()?.Description;
    }

    /// <summary>
    /// Combines two responses, keeping the failure if any
    /// </summary>
    public static Response<T> Combine<T>(this IResponse<T> first, IResponse second)
    {
        if (first.IsFailure)
            return (Response<T>)first;

        if (second.IsFailure)
            return new Response<T>((Response)second, first.Data);

        return (Response<T>)first;
    }

    /// <summary>
    /// Executes a function and returns response
    /// </summary>
    public static Response<TResult> Bind<T, TResult>(
        this IResponse<T> response,
        Func<T, Response<TResult>> func
    )
    {
        if (response.IsFailure)
        {
            return new Response<TResult>((Response)response, default(TResult));
        }

        return func(response.Data);
    }

    /// <summary>
    /// Gets metadata value by key
    /// </summary>
    public static TValue GetMetadata<TValue>(this IResponse response, string key)
    {
        if (response.Metadata != null && response.Metadata.TryGetValue(key, out var value))
        {
            if (value is TValue typedValue)
                return typedValue;
        }
        return default(TValue);
    }

    /// <summary>
    /// Checks if metadata contains key
    /// </summary>
    public static bool HasMetadata(this IResponse response, string key)
    {
        return response.Metadata != null && response.Metadata.ContainsKey(key);
    }
}