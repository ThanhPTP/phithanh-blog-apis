using Carter;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Modules.FileModules;

namespace PhiThanh.WebApi.Endpoints
{
    public class FileEndpoints : CarterModule
    {
        private const string TAG_ROUTE = "file";
        public FileEndpoints() : base($"api/v{{version:apiVersion}}/{TAG_ROUTE}")
        {
            WithTags(TAG_ROUTE);
            RequireAuthorization();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.AddEndpointsWithFromForm<UploadFilesRequest>("/upload-files")
                .Produces(200, typeof(ApiResponse));
        }
    }
}
