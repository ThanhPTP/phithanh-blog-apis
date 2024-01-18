using PhiThanh.Core;
using PhiThanh.DataAccess.Repositories;

namespace PhiThanh.Modules.CategoryModule
{
    public class DeleteBatchCategoriesRequest : BaseCommand
    {
        public List<Guid> Ids { get; set; } = [];
    }

    public class DeleteBatchCategoriesProfileMapper : AutoMapper.Profile
    {
        public DeleteBatchCategoriesProfileMapper()
        {
        }
    }

    public class DeleteBatchCategoriesValidator : BaseValidator<DeleteBatchCategoriesRequest>
    {
        public DeleteBatchCategoriesValidator()
        {
        }
    }

    public class DeleteBatchCategoriesHandler(ICategoryRepository categoryRepository) :
        ICommandHandler<DeleteBatchCategoriesRequest>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<ApiResponse> Handle(DeleteBatchCategoriesRequest request, CancellationToken cancellationToken)
        {
            await _categoryRepository.DeleteAsync(w => request.Ids.Contains(w.Id), true);
            return new ApiResponse();
        }
    }
}