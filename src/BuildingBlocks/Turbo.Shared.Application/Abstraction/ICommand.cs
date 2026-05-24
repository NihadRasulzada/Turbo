using Turbo.Shared.Application.ResponseObject.Concreate;

namespace Turbo.Shared.Application.Abstraction;

public interface ICommand<TResponse> where TResponse : Response { }
