using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Core.Utils;
using PhiThanh.Resources;
using System.Text.Json.Serialization;

namespace PhiThanh.Modules.AccountModule
{
    public class GetProfileRequest : BaseQuery<GetProfileResponse>
    {
        [JsonIgnore]
        public string? UserName { get; set; } = HttpContextUtils.Identity?.UserName();
    }

    public class GetProfileResponse
    {
        public int Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class GetProfileProfileMapper : AutoMapper.Profile
    {
        public GetProfileProfileMapper()
        {
        }
    }

    public class GetProfileValidator : BaseValidator<GetProfileRequest>
    {
        public GetProfileValidator()
        {
        }
    }

    public class GetProfileHandler() : IQueryHandler<GetProfileRequest, GetProfileResponse>
    {
        public async Task<ApiResponse<GetProfileResponse>> Handle(GetProfileRequest request, CancellationToken cancellationToken)
        {
            var rootUser = AppSettings.Instance.RootAccounts
                        .Find(f => string.Equals(f.UserName, request.UserName, StringComparison.OrdinalIgnoreCase));

            if (rootUser != null)
            {
                return await Task.FromResult(new ApiResponse<GetProfileResponse>(new GetProfileResponse
                {
                    FullName = rootUser.DisplayName,
                    Email = rootUser.Email,
                    Id = 0,
                    PhoneNumber = rootUser.PhoneNumber,
                    UserName = rootUser.UserName
                }));
            }

            ExceptionUtils.ThrowValidation(nameof(request.UserName),
                nameof(ValidationMessage.ERR_001_USER_NOT_EXISTS),
                ValidationMessage.ERR_001_USER_NOT_EXISTS);

            return new ApiResponse<GetProfileResponse>();
        }
    }
}
