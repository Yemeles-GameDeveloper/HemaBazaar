
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace HemaBazaar.MVC.Services
{
    public class RedisCacheService<T>
    {
        IDatabase _db;
        JsonSerializerOptions _serializerOptions;
        string _typePrefix;
        ILogger<RedisCacheService<T>> _logger;

        public RedisCacheService(
            IConnectionMultiplexer connectionMultiplexer,
            JsonSerializerOptions jsonOptions,
            ILogger<RedisCacheService<T>> logger)
        {
            _db = connectionMultiplexer.GetDatabase();
            _serializerOptions = jsonOptions;
            _typePrefix = typeof(T).Name + ":";
            _logger = logger;
        }

        string BuildKey(string key) => _typePrefix + key;

        public async Task<T?> GetAsync(string key)
        {
            try
            {
                var redisKey = BuildKey(key);
                var value = await _db.StringGetAsync(redisKey);

                if (value.IsNullOrEmpty)
                    return default;

                return JsonSerializer.Deserialize<T>(value, _serializerOptions);
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable during GetAsync for key '{Key}'. Returning default.", key);
                return default;
            }
        }

        public async Task SetAsync(string key, T value, TimeSpan expire)
        {
            try
            {
                var redisKey = BuildKey(key);
                var json = JsonSerializer.Serialize(value, _serializerOptions);
                await _db.StringSetAsync(redisKey, json, expire);
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable during SetAsync for key '{Key}'. Skipping cache write.", key);
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                var redisKey = BuildKey(key);
                return await _db.KeyDeleteAsync(redisKey);
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable during RemoveAsync for key '{Key}'.", key);
                return false;
            }
        }

        public async Task<bool> ExistAsync(string key)
        {
            try
            {
                var redisKey = BuildKey(key);
                return await _db.KeyExistsAsync(redisKey);
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable during ExistAsync for key '{Key}'.", key);
                return false;
            }
        }

        /// <summary>
        /// Returns cached value if available; otherwise invokes func(), stores the result (if Redis is up), and returns it.
        /// Falls back to func() directly when Redis is unavailable.
        /// </summary>
        public async Task<T> GetOrSetAsync(string key, Func<Task<T>> func, TimeSpan expire)
        {
            try
            {
                var cached = await GetAsync(key);
                if (cached != null)
                    return cached;

                var data = await func();
                await SetAsync(key, data, expire);
                return data;
            }
            catch (RedisException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable during GetOrSetAsync for key '{Key}'. Fetching data directly.", key);
                return await func();
            }
        }
    }
}
