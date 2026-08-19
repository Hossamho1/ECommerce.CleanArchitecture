namespace ECommerce.Infrastructure.Caching;

public sealed class CacheEnveolpe<T>
{
    public required T Payload { get; init; }

    public DateTime CreatedAtUtc { get; init; } 
    public DateTime LastAccessedAtUtc { get; set; }


}
