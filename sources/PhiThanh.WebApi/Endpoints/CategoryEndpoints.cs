using Carter;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Modules.CategoryModule;

namespace PhiThanh.Operation.WebApi.Endpoints
{
    public class CategoryEndpoints : CarterModule
    {
        private const string TAG_ROUTE = "category";
        public CategoryEndpoints() : base($"api/v{{version:apiVersion}}/{TAG_ROUTE}")
        {
            WithTags(TAG_ROUTE);
            RequireAuthorization();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.AddEndpoints<GetListCategoriesRequest>("/get-list-categories")
                .Produces(200, typeof(ApiResponse<PagingResult<GetListCategoriesResponse>>));

            app.AddEndpoints<GetDetailCategoryRequest>("/get-detail-category")
                .Produces(200, typeof(ApiResponse<GetDetailCategoryResponse>));

            app.AddEndpoints<CreateOrUpdateCategoryRequest>("/create-or-update-category")
                .Produces(200, typeof(ApiResponse));

            app.AddEndpoints<DeleteCategoryRequest>("/delete-category")
                .Produces(200, typeof(ApiResponse));

            app.AddEndpoints<DeleteBatchCategoriesRequest>("/delete-batch-categories")
                .Produces(200, typeof(ApiResponse));
        }
    }
}