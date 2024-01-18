using AutoMapper;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using System.Linq.Expressions;

namespace PhiThanh.Modules.TagModule
{
    public class GetListTagsRequest : BaseListQuery<PagingResult<GetListTagsResponse>>
    {
        public string? Key { get; set; }
    }

    public class GetListTagsResponse
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class GetListTagsProfileMapper : AutoMapper.Profile
    {
        public GetListTagsProfileMapper()
        {
            CreateMap<Tag, GetListTagsResponse>();
        }
    }

    public class GetListTagsValidator : BaseValidator<GetListTagsRequest>
    {
        public GetListTagsValidator()
        {
        }
    }

    public class GetListTagsHandler(ITagRepository tagRepository, IMapper mapper) :
        IQueryHandler<GetListTagsRequest, PagingResult<GetListTagsResponse>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITagRepository _tagRepository = tagRepository;

        public async Task<ApiResponse<PagingResult<GetListTagsResponse>>> Handle(GetListTagsRequest request, CancellationToken cancellationToken)
        {
            Expression<Func<Tag, bool>> predicate = x => !x.IsDeleted;

            if (!string.IsNullOrEmpty(request.Key))
            {
                string search = request.Key.ToLower();
                predicate = predicate.And(x => x.Key.ToLower().Contains(search));
            }

            var shops = await _tagRepository.GetAsync(predicate, request.FilterOptions);
            return new ApiResponse<PagingResult<GetListTagsResponse>>(
                _mapper.Map<PagingResult<GetListTagsResponse>>(shops)
            );
        }
    }
}