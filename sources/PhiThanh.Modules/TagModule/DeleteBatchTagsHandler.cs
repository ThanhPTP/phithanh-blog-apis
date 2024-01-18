using PhiThanh.Core;
using PhiThanh.DataAccess.Repositories;

namespace PhiThanh.Modules.TagModule
{
    public class DeleteBatchTagsRequest : BaseCommand
    {
        public List<Guid> Ids { get; set; } = [];
    }

    public class DeleteBatchTagsProfileMapper : AutoMapper.Profile
    {
        public DeleteBatchTagsProfileMapper()
        {
        }
    }

    public class DeleteBatchTagsValidator : BaseValidator<DeleteBatchTagsRequest>
    {
        public DeleteBatchTagsValidator()
        {
        }
    }

    public class DeleteBatchTagsHandler(ITagRepository tagRepository) :
        ICommandHandler<DeleteBatchTagsRequest>
    {
        private readonly ITagRepository _tagRepository = tagRepository;

        public async Task<ApiResponse> Handle(DeleteBatchTagsRequest request, CancellationToken cancellationToken)
        {
            await _tagRepository.DeleteAsync(w => request.Ids.Contains(w.Id), true);
            return new ApiResponse();
        }
    }
}