using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using System.Linq.Expressions;

namespace PhiThanh.Modules.PostModule
{
    public class GetListPostsRequest : BaseListQuery<PagingResult<GetListPostsResponse>>
    {
        public string? Title { get; set; }
    }

    public class GetListPostsResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public List<string> Tags { get; set; } = [];
        public string Category { get; set; }
        public DateTime? DisplayDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class GetListPostsProfileMapper : AutoMapper.Profile
    {
        public GetListPostsProfileMapper()
        {
            CreateMap<Post, GetListPostsResponse>()
                .ForMember(dest => dest.Category, src => src.MapFrom(p => p.Categories!.FirstOrDefault()!.Category!.Name))
                .ForMember(dest => dest.Tags, src => src.MapFrom(p => p.Tags!.Select(s => s.Tag!.Key)))
                .ForMember(dest => dest.Status, src => src.MapFrom(p => p.PostStatus.ToString()));
        }
    }

    public class GetListPostsValidator : BaseValidator<GetListPostsRequest>
    {
        public GetListPostsValidator()
        {
        }
    }

    public class GetListPostsHandler(IPostRepository postRepository, IMapper mapper) :
        IQueryHandler<GetListPostsRequest, PagingResult<GetListPostsResponse>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IPostRepository _postRepository = postRepository;

        public async Task<ApiResponse<PagingResult<GetListPostsResponse>>> Handle(GetListPostsRequest request, CancellationToken cancellationToken)
        {
            Expression<Func<Post, bool>> predicate = x => !x.IsDeleted;

            if (!string.IsNullOrEmpty(request.Title))
            {
                string search = request.Title.ToLower();
                predicate = predicate.And(x => x.Title.ToLower().Contains(search));
            }

            var posts = await _postRepository.GetAsync(predicate, request.FilterOptions,
                i => i.Include(a => a.Categories)
                .ThenInclude(a => a.Category)
                .Include(a => a.Tags)
                .ThenInclude(a => a.Tag));

            return new ApiResponse<PagingResult<GetListPostsResponse>>(
                _mapper.Map<PagingResult<GetListPostsResponse>>(posts)
            );
        }
    }
}