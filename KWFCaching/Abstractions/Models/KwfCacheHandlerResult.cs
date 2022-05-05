namespace KWFCaching.Abstractions.Models
{
    using KWFCaching.Abstractions.Interfaces;

    public class KwfCacheHandlerResult<TResult> : IKwfCacheHandlerResult<TResult>
        where TResult : class
    {
        protected KwfCacheHandlerResult(TResult result)
        {
            this.Result = result;
        }

        protected KwfCacheHandlerResult(TResult result, bool preventCacheWrite)
        {
            this.Result = result;
            this.PreventCacheWrite = preventCacheWrite;
        }

        public bool PreventCacheWrite { get; init; }

        public TResult Result { get; init; }

        public static KwfCacheHandlerResult<TResult> ReturnResult(TResult result)
        {
            return new KwfCacheHandlerResult<TResult>(result);
        }

        public static KwfCacheHandlerResult<TResult> ReturnNotCachedResult(TResult result)
        {
            return new KwfCacheHandlerResult<TResult>(result, true);
        }
    }
}
