using Carter;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Modules.PostModule;

namespace PhiThanh.Operation.WebApi.Endpoints
{
    public class PostEndpoints : CarterModule
    {
        private const string TAG_ROUTE = "post";
        public PostEndpoints() : base($"api/v{{version:apiVersion}}/{TAG_ROUTE}")
        {
            WithTags(TAG_ROUTE);
            RequireAuthorization();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.AddEndpoints<GetListPostsRequest>("/get-list-posts")
                .Produces(200, typeof(ApiResponse<PagingResult<GetListPostsResponse>>));

            app.AddEndpoints<GetDetailPostRequest>("/get-detail-post")
                .Produces(200, typeof(ApiResponse<GetDetailPostResponse>));

            app.AddEndpoints<CreateOrUpdatePostRequest>("/create-or-update-post")
                .Produces(200, typeof(ApiResponse));

            app.AddEndpoints<DeletePostRequest>("/delete-post")
                .Produces(200, typeof(ApiResponse));

            app.AddEndpoints<DeleteBatchPostsRequest>("/delete-batch-posts")
                .Produces(200, typeof(ApiResponse));
        }
    }
}