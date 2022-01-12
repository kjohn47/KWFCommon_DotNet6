namespace KWFCaching.Memory.Interfaces
{
    public interface IMemoryCacheHandlerResult<TResult> where TResult : class
    {
        bool PreventCacheWrite { get; }
        TResult Result { get; }
    }
}
