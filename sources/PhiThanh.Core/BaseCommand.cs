using MediatR;

namespace PhiThanh.Core
{
    public abstract class BaseCommand : ICommand
    {
    }

    public interface ICommand : IRequest<ApiResponse>
    {
    }
}
