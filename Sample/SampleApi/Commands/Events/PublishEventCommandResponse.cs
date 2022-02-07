namespace Sample.SampleApi.Commands.Events
{
    using KWFWebApi.Abstractions.Command;

    public class PublishEventCommandResponse : ICommandResponse
    {
        public PublishEventCommandResponse(Guid publishedId)
        {
            PublishedId = publishedId;
        }

        public Guid PublishedId { get; set; } = Guid.Empty;
    }
}
