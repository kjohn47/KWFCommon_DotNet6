namespace Sample.SampleApi.Commands.Events
{
    using KWFWebApi.Abstractions.Command;

    public class PublishEventCommandResponse : ICommandResponse
    {
        public PublishEventCommandResponse(string topic)
        {
            PublishedToTopic = topic;
        }

        public string PublishedToTopic { get; set; } = string.Empty;
    }
}
