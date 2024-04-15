namespace KWFEventBus.KWFKafka.Models
{
    using System;

    public class KwfKafkaBusException : Exception
    {
        public KwfKafkaBusException(string code, string message, string? reason = null) : base(message)
        {
            Code = code;
            Reason = reason;
        }

        public KwfKafkaBusException(string code, string message, Exception? innerEx, string? reason = null) : base(message, innerEx)
        {
            Code = code;
            Reason = reason;
        }

        public string Code;
        public string? Reason;
    }
}
