using Carter;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Modules.TagModule;

namespace PhiThanh.Operation.WebApi.Endpoints
{
    public class TagEndpoints : CarterModule
    {
        private const string TAG_ROUTE = "tag";
        public TagEndpoints() : base($"api/v{{version:apiVersion}}/{TAG_ROUTE}")
        {
            WithTags(TAG_ROUTE);
            RequireAuthorization();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.AddEndpoints<GetListTagsRequest>("/get-list-tags")
                .Produces(200, typeof(ApiResponse<PagingResult<GetListTagsResponse>>));

            app.AddEndpoints<GetDetailTagRequest>("/get-detail-tag")
                .Produces(200, typeof(ApiResponse<GetDetailTagResponse>));

            app.AddEndpoints<CreateOrUpdateTagRequest>("/create-or-update-tag")
                .Produces(200, typeof(ApiResponse));

            app.AddEndpoints<DeleteTagRequest>("/delete-tag")
                .Produces(200, typeof(ApiResponse));

            app.AddEndpoints<DeleteBatchTagsRequest>("/delete-batch-tags")
                .Produces(200, typeof(ApiResponse));
        }
    }
}