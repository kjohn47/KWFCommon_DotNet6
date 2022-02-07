namespace Sample.SampleApi.Commands.Events
{
    using KWFWebApi.Abstractions.Command;

    public class PublishEventCommandRequest : ICommandRequest
    {
        public string EventMessage { get; set; } = string.Empty;
    }
}
