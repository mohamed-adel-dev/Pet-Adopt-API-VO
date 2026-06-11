using System;
using System.Collections.Generic;
using System.Text;

namespace PetAdopt.BLL.Services.Interfaces.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string cacheKey);
        Task SetAsync<T>(string key, T value, TimeSpan expiry);
        Task RemoveAsync(string key);
    }
}
