using PhiThanh.Core;
using PhiThanh.DataAccess.Repositories;

namespace PhiThanh.Modules.PostModule
{
    public class DeletePostRequest : BaseCommand
    {
        public Guid Id { get; set; }
    }

    public class DeletePostProfileMapper : AutoMapper.Profile
    {
        public DeletePostProfileMapper()
        {
        }
    }

    public class DeletePostValidator : BaseValidator<DeletePostRequest>
    {
        public DeletePostValidator()
        {
        }
    }

    public class DeletePostHandler(IPostRepository postRepository) :
        ICommandHandler<DeletePostRequest>
    {
        private readonly IPostRepository _postRepository = postRepository;

        public async Task<ApiResponse> Handle(DeletePostRequest request, CancellationToken cancellationToken)
        {
            await _postRepository.DeleteAsync(request.Id, true);
            return new ApiResponse();
        }
    }
}