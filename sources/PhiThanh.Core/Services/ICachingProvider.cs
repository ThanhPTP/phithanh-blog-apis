namespace PhiThanh.Core.Services
{
    public interface ICachingProvider
    {
        Task<object?> GetValuesAsync(string key);
        Task SetValuesAsync(string key, object value);
        Task RemoveAsync(string key);
        Task<bool> IsExists(string key);
    }
}
