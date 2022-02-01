namespace KWFCaching.Memory.Implementation
{
    using KWFCaching.Abstractions.Models;

    using System.Collections.Generic;

    public class KwfCacheConfiguration
    {
        public CacheTimeSettings? CleanupInterval { get; set; }

        public CacheTimeSettings? DefaultCacheExpiration { get; set; }

        public KwfCacheSizeSettings? CacheSizeSettings { get; set; }

        public IDictionary<string, CacheKeyEntry>? CacheKeySettings { get; set; }
    }
}
