using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using PetAdopt.BLL.Services.Interfaces.Caching;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PetAdopt.BLL.Services.Implementations.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string cacheKey)
        {
            var data = await _cache.GetStringAsync(cacheKey);

            return data is null
                ? default
                : JsonSerializer.Deserialize<T>(data);
        }
        public async Task SetAsync<T>( string key, T value, TimeSpan expiry)
        {
            await _cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry
                });
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
