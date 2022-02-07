namespace Sample.SampleApi.Queries.Events
{
    using KWFWebApi.Abstractions.Query;

    public class GetEventsQueryRequest : IQueryRequest
    {
        public GetEventsQueryRequest()
        {            
        }

        public GetEventsQueryRequest(Guid eventId)
        {
            EventId = eventId;
        }

        public Guid? EventId { get; set; }
    }
}
