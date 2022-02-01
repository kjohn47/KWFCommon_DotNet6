namespace KWFCaching.Redis.Implementation
{
    using System;

    public class KwfRedisCacheException : Exception
    {
        public string ErrorCode { get; private set; }

        public KwfRedisCacheException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public KwfRedisCacheException(string errorCode, string message, Exception innerEx) : base(message, innerEx)
        {
            ErrorCode = errorCode;
        }
    }
}
