namespace KWFCaching.Memory.Implementation
{
    using KWFCaching.Memory.Interfaces;

    public class MemoryCacheHandlerResult<TResult> : IMemoryCacheHandlerResult<TResult>
        where TResult : class
    {
        protected MemoryCacheHandlerResult(TResult result)
        {
            this.Result = result;
        }

        protected MemoryCacheHandlerResult(TResult result, bool preventCacheWrite)
        {
            this.Result = result;
            this.PreventCacheWrite = preventCacheWrite;
        }

        public bool PreventCacheWrite { get; private set; }

        public TResult Result { get; private set; }

        public static MemoryCacheHandlerResult<TResult> ReturnResult(TResult result)
        {
            return new MemoryCacheHandlerResult<TResult>(result);
        }

        public static MemoryCacheHandlerResult<TResult> ReturnNotCachedResult(TResult result)
        {
            return new MemoryCacheHandlerResult<TResult>(result, true);
        }
    }
}
