using Dispo.Barber.Domain.Utils.interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Dispo.Barber.Domain.Utils
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Add a JSON to memory cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiration"></param>
        public void Add(string key, string obj, long expiration = 300) // 5 minutos por padrão
        {
            _memoryCache.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
            {
                _memoryCache.Set(key, obj, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiration)
                });
            }
        }

        /// <summary>
        /// Get a JSON from memory cache.
        /// Note: You'll need to deserialize your object after getting it from cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            _memoryCache.TryGetValue(key, out string value);
            return value;
        }

        /// <summary>
        /// Remove a JSON permanently from memory cache.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
