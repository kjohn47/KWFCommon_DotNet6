namespace KWFEventBus.KWFKafka.Models
{
    using System;

    internal class KwfKafkaBusException : Exception
    {
        public KwfKafkaBusException(string code, string message) : base(message)
        {
                Code = code;
        }

        public KwfKafkaBusException(string code, string message, Exception innerEx) : base(message, innerEx)
        {
            Code = code;
        }

        public string Code;
    }
}
