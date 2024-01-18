using MediatR;

namespace PhiThanh.Core
{
    public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest, ApiResponse>
    where TRequest : ICommand
    {
    }
}
