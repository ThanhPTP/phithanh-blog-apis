using MediatR;

namespace PhiThanh.Core
{
    public abstract class BaseQuery<TResponse> : IQuery<TResponse>
    {
    }

    public abstract class BaseListQuery<TResponse> : IQuery<TResponse>
    {
        public PagingFilterOption FilterOptions { get; set; } = new();
    }

    public interface IQuery<TResponse> : IRequest<ApiResponse<TResponse>>
    {
    }
}
