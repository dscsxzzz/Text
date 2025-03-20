using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Caching.Memory;

namespace MainAPI.Services;

public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void SetWithExpiration(string key, object value, TimeSpan expiration)
    {
        _cache.Set(key, value, expiration);
    }

    public T GetFromCache<T>(string key)
    {
        var data = _cache.Get<T>(key);
        return data;
    }

    public void RemoveFromCache(string key)
    {
        _cache.Remove(key);
    }
}
