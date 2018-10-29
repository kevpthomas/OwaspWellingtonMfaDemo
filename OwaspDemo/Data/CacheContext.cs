using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using OwaspDemo.Abstractions;
using OwaspDemo.Models;

namespace OwaspDemo.Data
{
    public class CacheContext : ICacheContext
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "TwoFactorTokenCacheKey";

        public CacheContext(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<UserToken> FindTokenByUserIdAsync(string userId)
        {
            return await Task.Run(() =>
            {
                var userTokens = GetUserTokensFromCache();

                var token = userTokens.SingleOrDefault(x =>
                    x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase));

                return token;
            });
        }

        public async Task<UserToken> AddAsync(string userId, string value)
        {
            return await Task.Run(() =>
            {
                var userTokens = GetUserTokensFromCache();
                
                var existingToken = userTokens.SingleOrDefault(x =>
                    x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase));

                if (existingToken != null) userTokens.Remove(existingToken);

                var token = new UserToken(userId, value);

                userTokens.Add(token);

                return token;
            });
        }

        private List<UserToken> GetUserTokensFromCache()
        {
            if (!_cache.TryGetValue(CacheKey, out List<UserToken> userTokens))
            {
                userTokens = new List<UserToken>();
                _cache.Set(CacheKey, userTokens, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)));
            }

            return userTokens;
        }
    }
}