namespace KWFCaching.Abstractions.Models
{
    public class CachedResult<TResult>
        where TResult : class
    {
        public TResult Result { get; set; } = default!;

        public bool CacheMiss { get; set; }
    }
}
