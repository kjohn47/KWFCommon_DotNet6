namespace KWFCaching.Memory.Implementation
{
    using System;

    public class KwfOnMemoryCacheException : Exception
    {
        public KwfOnMemoryCacheException(string code, string message) : base(message)
        {
            this.Code = code;
        }

        public string Code { get; set; }
    }
}
