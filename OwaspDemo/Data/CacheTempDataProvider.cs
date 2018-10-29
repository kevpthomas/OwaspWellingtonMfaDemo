using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;

namespace OwaspDemo.Data
{
    /// <summary>
    /// Added this implementation because the asp.net core CookieTempDataProvider
    /// implementation was failing unexpectedly with a Key Not Found exception.
    /// </summary>
    public class CacheTempDataProvider : ITempDataProvider
    {
        private readonly IMemoryCache _cache;

        private const string CacheKey = "CacheTempDataProviderCacheKey";

        public CacheTempDataProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            if (!_cache.TryGetValue(CacheKey, out IDictionary<string, object> tempData))
            {
                tempData = new Dictionary<string, object>();
                SaveTempData(context, tempData);
            }

            return tempData;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            _cache.Set(CacheKey, values, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));
        }
    }
}