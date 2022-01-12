﻿namespace KWFCaching.Memory.Implementation
{
    public class CachedResult<TResult>
        where TResult : class
    {
        public TResult? Result { get; set; }

        public bool CacheMiss { get; set; }
    }
}
