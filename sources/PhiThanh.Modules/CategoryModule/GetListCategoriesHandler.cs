using AutoMapper;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using System.Linq.Expressions;

namespace PhiThanh.Modules.CategoryModule
{
    public class GetListCategoriesRequest : BaseListQuery<PagingResult<GetListCategoriesResponse>>
    {
        public string? Name { get; set; }
    }

    public class GetListCategoriesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Slug { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class GetListCategoriesProfileMapper : AutoMapper.Profile
    {
        public GetListCategoriesProfileMapper()
        {
            CreateMap<Category, GetListCategoriesResponse>();
        }
    }

    public class GetListCategoriesValidator : BaseValidator<GetListCategoriesRequest>
    {
        public GetListCategoriesValidator()
        {
        }
    }

    public class GetListCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper) :
        IQueryHandler<GetListCategoriesRequest, PagingResult<GetListCategoriesResponse>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<ApiResponse<PagingResult<GetListCategoriesResponse>>> Handle(GetListCategoriesRequest request, CancellationToken cancellationToken)
        {
            Expression<Func<Category, bool>> predicate = x => !x.IsDeleted;

            if (!string.IsNullOrEmpty(request.Name))
            {
                string search = request.Name.ToLower();
                predicate = predicate.And(x => x.Name.ToLower().Contains(search));
            }

            var shops = await _categoryRepository.GetAsync(predicate, request.FilterOptions);
            return new ApiResponse<PagingResult<GetListCategoriesResponse>>(
                _mapper.Map<PagingResult<GetListCategoriesResponse>>(shops)
            );
        }
    }
}