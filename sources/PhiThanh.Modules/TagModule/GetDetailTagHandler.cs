using AutoMapper;
using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using PhiThanh.Resources;

namespace PhiThanh.Modules.TagModule
{
    public class GetDetailTagRequest : BaseQuery<GetDetailTagResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetDetailTagResponse
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class GetDetailTagProfileMapper : AutoMapper.Profile
    {
        public GetDetailTagProfileMapper()
        {
            CreateMap<Tag, GetDetailTagResponse>();
        }
    }

    public class GetDetailTagValidator : BaseValidator<GetDetailTagRequest>
    {
        public GetDetailTagValidator()
        {
        }
    }

    public class GetDetailTagHandler(ITagRepository tagRepository, IMapper mapper) :
        IQueryHandler<GetDetailTagRequest, GetDetailTagResponse>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITagRepository _tagRepository = tagRepository;

        public async Task<ApiResponse<GetDetailTagResponse>> Handle(GetDetailTagRequest request, CancellationToken cancellationToken)
        {
            var shop = await _tagRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id);
            if (shop == null)
            {
                ExceptionUtils.ThrowValidation(nameof(request.Id),
                           nameof(ValidationMessage.ERR_004_DATA_NOT_EXISTS),
                           ValidationMessage.ERR_004_DATA_NOT_EXISTS);
            }

            return new ApiResponse<GetDetailTagResponse>(
                _mapper.Map<GetDetailTagResponse>(shop)
            );
        }
    }
}