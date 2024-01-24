using AutoMapper;
using FluentValidation;
using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Repositories;
using PhiThanh.Resources;

namespace PhiThanh.Modules.CategoryModule
{
    public class CreateOrUpdateCategoryRequest : BaseCommand
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public int Order { get; set; }
        public Guid? ParentCategoryId { get; set; }
    }

    public class CreateOrUpdateCategoryProfileMapper : AutoMapper.Profile
    {
        public CreateOrUpdateCategoryProfileMapper()
        {
            CreateMap<CreateOrUpdateCategoryRequest, Category>();
        }
    }

    public class CreateOrUpdateCategoryValidator : BaseValidator<CreateOrUpdateCategoryRequest>
    {
        public CreateOrUpdateCategoryValidator()
        {
            RuleFor(a => a.Name).NotEmpty().NotNull();
        }
    }

    public class CreateOrUpdateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper) :
        ICommandHandler<CreateOrUpdateCategoryRequest>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<ApiResponse> Handle(CreateOrUpdateCategoryRequest request, CancellationToken cancellationToken)
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

        private async Task<Category> InsertAsync(CreateOrUpdateCategoryRequest request)
        {
            var entity = _mapper.Map<Category>(request);

            if (entity.ParentCategoryId != null)
            {
                var parentCategory = await _categoryRepository.GetFirstOrDefaultAsync(f => f.Id == request.ParentCategoryId!.Value);
                if (parentCategory != null)
                {
                    entity.ParentCategoryId = request.ParentCategoryId;
                    entity.Level = parentCategory.Level + 1;
                }
            }
            else
            {
                entity.Level = 0;
            }

            await _categoryRepository.InsertAsync(entity, true);
            return entity;
        }

        private async Task<Category> UpdateAsync(CreateOrUpdateCategoryRequest request)
        {
            var entity = await _categoryRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id!.Value);

            if (entity == null)
            {
                ExceptionUtils.ThrowValidation(nameof(request.Id),
                           nameof(ValidationMessage.ERR_004_DATA_NOT_EXISTS),
                           ValidationMessage.ERR_004_DATA_NOT_EXISTS);

                return new Category();
            }

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Slug = request.Slug;
            entity.Order = request.Order;
            entity.BannerUrl = request.BannerUrl;

            if (entity.ParentCategoryId != null)
            {
                var parentCategory = await _categoryRepository.GetFirstOrDefaultAsync(f => f.Id == request.ParentCategoryId!.Value);
                if (parentCategory != null)
                {
                    entity.ParentCategoryId = request.ParentCategoryId;
                    entity.Level = parentCategory.Level + 1;
                }
            }
            else
            {
                entity.Level = 0;
            }

            await _categoryRepository.UpdateAsync(entity, true);
            return entity;
        }
    }
}