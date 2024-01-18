using AutoMapper;
using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using PhiThanh.Resources;

namespace PhiThanh.Modules.CategoryModule
{
    public class GetDetailCategoryRequest : BaseQuery<GetDetailCategoryResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetDetailCategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public int Level { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class GetDetailCategoryProfileMapper : AutoMapper.Profile
    {
        public GetDetailCategoryProfileMapper()
        {
            CreateMap<Category, GetDetailCategoryResponse>();
        }
    }

    public class GetDetailCategoryValidator : BaseValidator<GetDetailCategoryRequest>
    {
        public GetDetailCategoryValidator()
        {
        }
    }

    public class GetDetailCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper) :
        IQueryHandler<GetDetailCategoryRequest, GetDetailCategoryResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<ApiResponse<GetDetailCategoryResponse>> Handle(GetDetailCategoryRequest request, CancellationToken cancellationToken)
        {
            var shop = await _categoryRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);
            if (shop == null)
            {
                ExceptionUtils.ThrowValidation(nameof(request.Id),
                           nameof(ValidationMessage.ERR_004_DATA_NOT_EXISTS),
                           ValidationMessage.ERR_004_DATA_NOT_EXISTS);
            }

            return new ApiResponse<GetDetailCategoryResponse>(
                _mapper.Map<GetDetailCategoryResponse>(shop)
            );
        }
    }
}