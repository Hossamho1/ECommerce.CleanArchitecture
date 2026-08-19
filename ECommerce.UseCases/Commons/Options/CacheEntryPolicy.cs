using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Infrastructure.Caching;

public class CacheEntryPolicy
{
    public int AbsoluteExpirationDays { get; set; } = 30;

    public int SlidingExpirationDays { get; set; } = 7;

    public int LocalCacheExpirationMinutes { get; set; } = 5;

    public int SlidingRefreshThresholdMinutes { get; set; } = 10;
}
