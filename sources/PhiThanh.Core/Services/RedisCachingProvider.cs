using Newtonsoft.Json;
using StackExchange.Redis;

namespace PhiThanh.Core.Services
{
    public class RedisCachingProvider : ICachingProvider
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        public RedisCachingProvider(string endpoints, string password, bool ssl)
        {
            _redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { endpoints },
                Ssl = ssl,
                Password = password
            });

            _database = _redis.GetDatabase();
        }

        public async Task<object?> GetValuesAsync(string key)
        {
            var value = await _database.StringGetAsync(new RedisKey(key));
            return value;
        }

        public async Task<bool> IsExists(string key)
        {
            var result = await _database.KeyExistsAsync(new RedisKey(key));
            return await Task.FromResult(result);
        }

        public async Task SetValuesAsync(string key, object value)
        {
            await _database.StringSetAsync(new RedisKey(key), new RedisValue(JsonConvert.SerializeObject(value)));
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(new RedisKey(key));
        }
    }
}
