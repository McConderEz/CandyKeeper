using CandyKeeper.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CandyKeeper.Application.Services;

public class UserSessionService: IUserSessionService
{
    private readonly IMemoryCache _memoryCache;

    public UserSessionService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    public  void SetUserData(string key, object value, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromHours(1)
        };
        
        _memoryCache.Set(key, value, cacheEntryOptions);
    }

    public T GetUserData<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T value))
        {
            return value;
        }
        else
        {
            return default;
        }
    }
}