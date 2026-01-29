
using System.Text.Json;
using StackExchange.Redis;

namespace User_prueba
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDb;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key) 
        {
            var value = await _redisDb.StringGetAsync(key);
            if (value.IsNullOrEmpty) 
                return default;
            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task RemoveDataAsync(string key)
        {
             await _redisDb.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _redisDb.StringSetAsync(key, json, expiration);
        }
    }
}