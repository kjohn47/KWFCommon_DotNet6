namespace Sample.SampleApi.Models
{
    public class KwfEvent
    {
        public KwfEvent()
        {

        }

        public KwfEvent(string message)
        {
            Message = message;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;
    }
}
