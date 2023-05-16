namespace KWFEventBus.KWFRabbitMQ.Models
{
    using System;

    public class KwfRabbitMQException : Exception
    {
        public KwfRabbitMQException(string code, string message) : base(message)
        {
            Code = code;
        }

        public KwfRabbitMQException(string code, string message, Exception innerEx) : base(message, innerEx)
        {
            Code = code;
        }

        public string Code;
    }
}
