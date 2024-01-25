using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.DataAccess.Entities;
using PhiThanh.DataAccess.Kernel;
using PhiThanh.DataAccess.Repositories;
using PhiThanh.Resources;
using Serilog;
using static PhiThanh.Core.Constants;

namespace PhiThanh.Modules.PostModule
{
    public class CreateOrUpdatePostRequest : BaseCommand
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string BannerUrl { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime? DisplayDate { get; set; }
        public List<string> Tags { get; set; } = [];
        public bool IsDraft { get; set; }
    }

    public class CreateOrUpdatePostProfileMapper : AutoMapper.Profile
    {
        public CreateOrUpdatePostProfileMapper()
        {
            CreateMap<CreateOrUpdatePostRequest, Post>()
                .ForMember(x => x.Tags, opt => opt.Ignore());
        }
    }

    public class CreateOrUpdatePostValidator : BaseValidator<CreateOrUpdatePostRequest>
    {
        public CreateOrUpdatePostValidator()
        {
        }
    }

    public class CreateOrUpdatePostHandler(IPostRepository postRepository,
        ITagRepository tagRepository, IUnitOfWork uow, IMapper mapper, ILogger logger,
        ICategoryRepository categoryRepository, IPostCategoryRepository postCategoryRepository,
        IPostTagRepository postTagRepository) :
        ICommandHandler<CreateOrUpdatePostRequest>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IPostRepository _postRepository = postRepository;
        private readonly ITagRepository _tagRepository = tagRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IPostCategoryRepository _postCategoryRepository = postCategoryRepository;
        private readonly IPostTagRepository _postTagRepository = postTagRepository;
        private readonly IUnitOfWork _uow = uow;
        private readonly ILogger _logger = logger;

        public async Task<ApiResponse> Handle(CreateOrUpdatePostRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _uow.BeginTransaction();
                if (request.Id == null)
                {
                    await InsertAsync(request);
                }
                else
                {
                    await UpdateAsync(request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                _uow.RollBackTransaction();
            }
            finally
            {
                _uow.CommitTransaction();
            }
            return new ApiResponse();
        }

        private async Task<Post> InsertAsync(CreateOrUpdatePostRequest request)
        {
            var entity = _mapper.Map<Post>(request);
            if (request.IsDraft)
            {
                entity.PostStatus = PostStatus.Draft;
            }
            else
            {
                entity.PostStatus = PostStatus.Publish;
            }

            if (request.DisplayDate == null)
            {
                request.DisplayDate = DateTime.Now;
            }

            await _postRepository.InsertAsync(entity);

            if (request.Tags.Count > 0)
            {
                var existedTags = await _tagRepository.GetListAsync(f => request.Tags.Contains(f.Key));
                var tagKeysNotExisted = request.Tags.Except(existedTags.Select(f => f.Key)).ToList();
                var notExistedTags = tagKeysNotExisted.ConvertAll(s => new Tag
                {
                    Key = s
                });

                await _tagRepository.InsertAsync(notExistedTags);
                await _uow.SaveChangesAsync();
                entity.Tags = existedTags.Union(notExistedTags).Select(s => new PostTag
                {
                    TagId = s.Id,
                    PostId = entity.Id
                }).ToHashSet();
            }

            var category = await _categoryRepository.GetFirstOrDefaultAsync(f => f.Id == request.CategoryId);
            if (category != null)
            {
                entity.Categories = [
                    new() { CategoryId = category.Id, PostId = entity.Id }
                ];
            }

            await _postRepository.UpdateAsync(entity);
            await _uow.SaveChangesAsync();
            return entity;
        }

        private async Task<Post> UpdateAsync(CreateOrUpdatePostRequest request)
        {
            var entity = await _postRepository.GetFirstOrDefaultAsync(f => f.Id == request.Id!.Value,
                include: i => i.Include(a => a.Tags)
                            .ThenInclude(a => a.Tag)
                            .Include(a => a.Categories)
                            .ThenInclude(a => a.Category));

            if (entity == null)
            {
                ExceptionUtils.ThrowValidation(nameof(request.Id),
                           nameof(ValidationMessage.ERR_004_DATA_NOT_EXISTS),
                           ValidationMessage.ERR_004_DATA_NOT_EXISTS);

                return new Post();
            }

            if (request.IsDraft)
            {
                entity.PostStatus = PostStatus.Draft;
            }
            else
            {
                entity.PostStatus = PostStatus.Publish;
            }

            if (request.DisplayDate == null)
            {
                request.DisplayDate = DateTime.Now;
            }

            if (entity.Tags!.Count > 0)
            {
                await _postTagRepository.DeleteAsync(entity.Tags);
            }

            if (entity.Categories!.Count > 0)
            {
                await _postCategoryRepository.DeleteAsync(entity.Categories);
            }

            if (request.Tags.Count > 0)
            {
                var existedTags = await _tagRepository.GetListAsync(f => request.Tags.Contains(f.Key));
                var tagKeysNotExisted = request.Tags.Except(existedTags.Select(f => f.Key)).ToList();
                var notExistedTags = tagKeysNotExisted.ConvertAll(s => new Tag
                {
                    Key = s
                });

                entity.Tags = existedTags.Union(notExistedTags).Select(s => new PostTag
                {
                    TagId = s.Id,
                    PostId = entity.Id
                }).ToHashSet();
            }

            var category = await _categoryRepository.GetFirstOrDefaultAsync(f => f.Id == request.CategoryId);
            if (category != null)
            {
                entity.Categories = [
                    new() { CategoryId = category.Id, PostId = entity.Id }
                ];
            }

            await _postRepository.UpdateAsync(entity);
            await _uow.SaveChangesAsync();
            return entity;
        }
    }
}