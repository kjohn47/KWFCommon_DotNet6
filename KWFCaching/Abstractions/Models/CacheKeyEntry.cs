namespace KWFCaching.Abstractions.Models
{
    public class CacheKeyEntry
    {
        public bool NoExpiration { get; set; }
        public CacheTimeSettings? Expiration { get; set; }
    }
}
