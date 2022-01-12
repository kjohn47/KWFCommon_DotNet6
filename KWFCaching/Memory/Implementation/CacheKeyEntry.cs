namespace KWFCaching.Memory.Implementation
{
    public class CacheKeyEntry
    {
        public bool NoExpiration { get; set; }
        public CacheTimeSettings? Expiration { get; set; }
    }
}
