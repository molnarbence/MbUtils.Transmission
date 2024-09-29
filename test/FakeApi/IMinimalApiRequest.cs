using MediatR;

namespace FakeApi;

public interface IMinimalApiRequest : IRequest<IResult>
{
}

public interface IMinimalApiRequestHandler<TRequest> : IRequestHandler<TRequest, IResult> where TRequest : IMinimalApiRequest
{
}

