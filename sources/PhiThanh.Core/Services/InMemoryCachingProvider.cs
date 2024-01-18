using Microsoft.Extensions.Caching.Memory;

namespace PhiThanh.Core.Services
{
    public class InMemoryCachingProvider(IMemoryCache memoryCache) : ICachingProvider
    {
        private readonly IMemoryCache _memoryCache = memoryCache;

        public async Task<object?> GetValuesAsync(string key)
        {
            _memoryCache.TryGetValue(key, out var value);
            return await Task.FromResult(value);
        }

        public async Task<bool> IsExists(string key)
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public async Task SetValuesAsync(string key, object value)
        {
            _memoryCache.Set(key, value);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await Task.CompletedTask;
        }
    }
}
