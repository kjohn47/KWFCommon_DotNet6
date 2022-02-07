namespace Sample.SampleApi.Queries.Events
{
    using KWFWebApi.Abstractions.Query;

    using Sample.SampleApi.Events;

    public class GetEventsQueryResponse : IQueryResponse
    {
        public GetEventsQueryResponse()
        {
            SavedEvents = new List<KwfEvent>();
        }
        
        public GetEventsQueryResponse(IEnumerable<KwfEvent> savedEvents)
        {
            SavedEvents = savedEvents;
        }

        public IEnumerable<KwfEvent> SavedEvents { get; set; }
    }
}
