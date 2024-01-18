using Newtonsoft.Json;

namespace PhiThanh.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T? CastTo<T>(this object obj)
        {
            var tmpObj = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(tmpObj);
        }
    }
}
