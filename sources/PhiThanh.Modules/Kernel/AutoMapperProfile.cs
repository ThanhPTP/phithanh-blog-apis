using PhiThanh.Core;

namespace PhiThanh.Modules.Kernel
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap(typeof(PagingResult<>), typeof(PagingResult<>));
        }
    }
}
