using PhiThanh.Core;
using PhiThanh.DataAccess.Repositories;

namespace PhiThanh.Modules.PostModule
{
    public class DeleteBatchPostsRequest : BaseCommand
    {
        public List<Guid> Ids { get; set; } = [];
    }

    public class DeleteBatchPostsProfileMapper : AutoMapper.Profile
    {
        public DeleteBatchPostsProfileMapper()
        {
        }
    }

    public class DeleteBatchPostsValidator : BaseValidator<DeleteBatchPostsRequest>
    {
        public DeleteBatchPostsValidator()
        {
        }
    }

    public class DeleteBatchPostsHandler(IPostRepository postRepository) :
        ICommandHandler<DeleteBatchPostsRequest>
    {
        private readonly IPostRepository _postRepository = postRepository;

        public async Task<ApiResponse> Handle(DeleteBatchPostsRequest request, CancellationToken cancellationToken)
        {
            await _postRepository.DeleteAsync(w => request.Ids.Contains(w.Id), true);
            return new ApiResponse();
        }
    }
}