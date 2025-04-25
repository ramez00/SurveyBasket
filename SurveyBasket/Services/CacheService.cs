
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyBasket.Services;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public  async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(value))
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
    }
}
