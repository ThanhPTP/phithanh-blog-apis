using MediatR;

namespace PhiThanh.Core
{
    public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, ApiResponse<TResponse>>
    where TRequest : IQuery<TResponse>
    {
    }
}
