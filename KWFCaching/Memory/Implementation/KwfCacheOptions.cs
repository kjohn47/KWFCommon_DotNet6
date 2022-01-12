namespace KWFCaching.Memory.Implementation
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Caching.Memory;

    public class KwfCacheOptions : MemoryCacheOptions
    {
        public IDictionary<string, CacheKeyEntry>? CachedKeySettings { get; set; }
    }
}
