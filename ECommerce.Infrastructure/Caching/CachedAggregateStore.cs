using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Caching;

public class CachedAggregateStore<T>(
    HybridCache cache
    ,IOptionsMonitor<CacheEntryPolicy> options
    ) : ICachedAggregateStore<T>
{
    private readonly CacheEntryPolicy _options = options.Get(typeof(T).Name);
    public async Task<T?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        CacheEnveolpe<T?> envelope = await cache.TryGetAsync<CacheEnveolpe<T>>(key, cancellationToken);
        
        return envelope.Payload;
    }

    public async Task<T> GetOrCreateAsync(string key, Func<CancellationToken, Task<T>> factory, CancellationToken cancellationToken)
    {
        // GET Ruturn it (updatee lastaccess), Create

        var envelope = await cache.GetOrCreateAsync(
            key,
            async cancel =>
            {
                var value = await factory(cancel);
                var utcNow = DateTime.UtcNow;

                return new CacheEnveolpe<T> { Payload = value, CreatedAtUtc = utcNow, LastAccessedAtUtc = utcNow };
            },
            CreateEntryOptionsForNewEnvelop(),
            cancellationToken: cancellationToken
        );

        await RefershExpirationIfNeedAsync(key, envelope, cancellationToken);

        return envelope.Payload;
    }


    private TimeSpan? CalculateExpiration(
    DateTime createdAtUtc,
    DateTime lastAccessedUtc,
    DateTime utcNow
)
    {
        // sliding - Aboslute
        var absoluteRemaining = createdAtUtc
            .AddDays(_options.AbsoluteExpirationDays)
            .Subtract(utcNow);

        var slidingRemaining = lastAccessedUtc
            .AddDays(_options.SlidingExpirationDays)
            .Subtract(utcNow);

        if (absoluteRemaining <= TimeSpan.Zero || slidingRemaining <= TimeSpan.Zero)
            return null;

        return absoluteRemaining >= slidingRemaining ? absoluteRemaining : slidingRemaining;
    }

    private HybridCacheEntryOptions CreateEntryOptionsForNewEnvelop()
    {
        var utcNow = DateTime.UtcNow;

        var expiration = CalculateExpiration(utcNow, utcNow, utcNow) ?? throw new Exception();

        return CreateEntryOptions(expiration);
    }

    private HybridCacheEntryOptions CreateEntryOptions(TimeSpan expiration)
    {
        var localExpiration = TimeSpan.FromMinutes(_options.LocalCacheExpirationMinutes);

        if (localExpiration > expiration)
            localExpiration = expiration;

        return new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = localExpiration
        };
    }

    private async Task RefershExpirationIfNeedAsync(string key, CacheEnveolpe<T> envelope, CancellationToken cancellationToken)
    {
        var utc = DateTime.UtcNow;

        var age = utc - envelope.LastAccessedAtUtc;

        if (age < TimeSpan.FromMinutes(_options.SlidingRefreshThresholdMinutes))
            return;

        var refershed = new CacheEnveolpe<T>
        {
            Payload = envelope.Payload,
            CreatedAtUtc = envelope.CreatedAtUtc,
            LastAccessedAtUtc = utc
        };
        await SetOrRemoveIfExpiredAsync(key, refershed, cancellationToken);
    }
    private async Task SetOrRemoveIfExpiredAsync(string key, CacheEnveolpe<T> refershed, CancellationToken cancellationToken)
    {
        var expiration = CalculateExpiration(
            refershed.CreatedAtUtc,
            refershed.LastAccessedAtUtc,
            DateTime.UtcNow
        );

        if (expiration is null)
        {
            await cache.RemoveAsync(key, cancellationToken);
            return;
        }

        await cache.SetAsync(key, refershed, CreateEntryOptions(expiration.Value), cancellationToken: cancellationToken);
    }
    public async Task SetAsync(string key, T value, CancellationToken ct = default)
    {
        var existing = await cache.TryGetAsync<CacheEnveolpe<T>>(key, ct);

        var envelop = new CacheEnveolpe<T>
        {
            Payload = value,
            CreatedAtUtc = existing?.CreatedAtUtc ?? DateTime.UtcNow,
            LastAccessedAtUtc = DateTime.UtcNow
        };

        await SetOrRemoveIfExpiredAsync(key, envelop, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await cache.RemoveAsync(key, ct);
}
