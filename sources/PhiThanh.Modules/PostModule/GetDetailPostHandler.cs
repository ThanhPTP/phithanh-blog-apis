using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using PhiThanh.Resources;
using static PhiThanh.Core.Constants;

namespace PhiThanh.Modules.PostModule
{
    public class GetDetailPostRequest : BaseQuery<GetDetailPostResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetDetailPostResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public PostStatus PostStatus { get; set; }
        public DateTime? DisplayDate { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string BannerUrl { get; set; }
        public Guid CategoryId { get; set; }
        public List<string> Tags { get; set; } = [];
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class GetDetailPostProfileMapper : AutoMapper.Profile
    {
        public GetDetailPostProfileMapper()
        {
            CreateMap<Post, GetDetailPostResponse>()
                .ForMember(dest => dest.CategoryId, src => src.MapFrom(p => p.Categories!.FirstOrDefault()!.Category!.Id))
                .ForMember(dest => dest.Tags, src => src.MapFrom(p => p.Tags!.Select(s => s.Tag!.Key)));
        }
    }

    public class GetDetailPostValidator : BaseValidator<GetDetailPostRequest>
    {
        public GetDetailPostValidator()
        {
        }
    }

    public class GetDetailPostHandler(IPostRepository postRepository, IMapper mapper) :
        IQueryHandler<GetDetailPostRequest, GetDetailPostResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IPostRepository _postRepository = postRepository;

        public async Task<ApiResponse<GetDetailPostResponse>> Handle(GetDetailPostRequest request, CancellationToken cancellationToken)
        {
            var shop = await _postRepository
                .GetFirstOrDefaultAsync(f => f.Id == request.Id, include: i => i.Include(a => a.Tags)
                                                                                .ThenInclude(a => a.Tag)
                                                                                .Include(a => a.Categories)
                                                                                .ThenInclude(a => a.Category));
            if (shop == null)
            {
                ExceptionUtils.ThrowValidation(nameof(request.Id),
                           nameof(ValidationMessage.ERR_004_DATA_NOT_EXISTS),
                           ValidationMessage.ERR_004_DATA_NOT_EXISTS);
            }

            return new ApiResponse<GetDetailPostResponse>(
                _mapper.Map<GetDetailPostResponse>(shop)
            );
        }
    }
}