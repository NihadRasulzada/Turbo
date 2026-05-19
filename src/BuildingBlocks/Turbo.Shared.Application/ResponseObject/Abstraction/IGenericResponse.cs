namespace Turbo.Shared.Application.ResponseObject.Abstraction;

public interface IResponse<T> : IResponse
{
    T Data { get; set; }
}