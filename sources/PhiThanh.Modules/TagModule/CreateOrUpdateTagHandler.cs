using AutoMapper;
using FluentValidation;
using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using PhiThanh.Resources;

namespace PhiThanh.Modules.TagModule
{
    public class CreateOrUpdateTagRequest : BaseCommand
    {
        public Guid? Id { get; set; }
        public string Key { get; set; }
    }

    public class CreateOrUpdateTagProfileMapper : AutoMapper.Profile
    {
        public CreateOrUpdateTagProfileMapper()
        {
            CreateMap<CreateOrUpdateTagRequest, Tag>();
        }
    }

    public class CreateOrUpdateTagValidator : BaseValidator<CreateOrUpdateTagRequest>
    {
        public CreateOrUpdateTagValidator()
        {
            RuleFor(a => a.Key).NotEmpty().NotNull();
        }
    }

    public class CreateOrUpdateTagHandler(ITagRepository tagRepository, IMapper mapper) :
        ICommandHandler<CreateOrUpdateTagRequest>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITagRepository _tagRepository = tagRepository;

        public async Task<ApiResponse> Handle(CreateOrUpdateTagRequest request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                await InsertAsync(request);
            }
            else
            {
                await UpdateAsync(request);
            }
            return new ApiResponse();
        }

        private async Task<Tag> InsertAsync(CreateOrUpdateTagRequest request)
        {
            var entity = _mapper.Map<Tag>(request);
            await _tagRepository.InsertAsync(entity, true);
            return entity;
        }

        private async Task<Tag> UpdateAsync(CreateOrUpdateTagRequest request)
        {
            var entity = await _tagRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id!.Value);

            if (entity == null)
            {
                ExceptionUtils.ThrowValidation(nameof(request.Id),
                           nameof(ValidationMessage.ERR_004_DATA_NOT_EXISTS),
                           ValidationMessage.ERR_004_DATA_NOT_EXISTS);

                return new Tag();
            }

            entity.Key = request.Key;
            await _tagRepository.UpdateAsync(entity, true);
            return entity;
        }
    }
}