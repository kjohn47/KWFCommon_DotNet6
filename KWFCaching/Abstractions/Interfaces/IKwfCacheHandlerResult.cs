namespace KWFCaching.Abstractions.Interfaces
{
    public interface IKwfCacheHandlerResult<TResult> where TResult : class
    {
        bool PreventCacheWrite { get; }
        TResult Result { get; }
    }
}
