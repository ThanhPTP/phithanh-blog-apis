using PhiThanh.Core;
using PhiThanh.DataAccess.Repositories;

namespace PhiThanh.Modules.CategoryModule
{
    public class DeleteCategoryRequest : BaseCommand
    {
        public Guid Id { get; set; }
    }

    public class DeleteCategoryProfileMapper : AutoMapper.Profile
    {
        public DeleteCategoryProfileMapper()
        {
        }
    }

    public class DeleteCategoryValidator : BaseValidator<DeleteCategoryRequest>
    {
        public DeleteCategoryValidator()
        {
        }
    }

    public class DeleteCategoryHandler(ICategoryRepository categoryRepository) :
        ICommandHandler<DeleteCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<ApiResponse> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            await _categoryRepository.DeleteAsync(request.Id, true);
            return new ApiResponse();
        }
    }
}