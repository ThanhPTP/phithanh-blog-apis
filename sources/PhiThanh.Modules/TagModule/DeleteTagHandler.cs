using PhiThanh.Core;
using PhiThanh.DataAccess.Repositories;

namespace PhiThanh.Modules.TagModule
{
    public class DeleteTagRequest : BaseCommand
    {
        public Guid Id { get; set; }
    }

    public class DeleteTagProfileMapper : AutoMapper.Profile
    {
        public DeleteTagProfileMapper()
        {
        }
    }

    public class DeleteTagValidator : BaseValidator<DeleteTagRequest>
    {
        public DeleteTagValidator()
        {
        }
    }

    public class DeleteTagHandler(ITagRepository tagRepository) :
        ICommandHandler<DeleteTagRequest>
    {
        private readonly ITagRepository _tagRepository = tagRepository;

        public async Task<ApiResponse> Handle(DeleteTagRequest request, CancellationToken cancellationToken)
        {
            await _tagRepository.DeleteAsync(request.Id, true);
            return new ApiResponse();
        }
    }
}