namespace PhiThanh.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName) != null;
        }
    }
}
