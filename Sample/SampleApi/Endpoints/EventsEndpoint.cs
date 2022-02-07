namespace Sample.SampleApi.Endpoints
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    using Sample.SampleApi.Commands.Events;
    using Sample.SampleApi.Queries.Events;

    public class EventsEndpoint : IEndpointConfiguration
    {
        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IConfiguration configuration)
        {
            builder.AddGet<GetEventsQueryResponse>(r =>
                r.SetRoute("get-all")
                 .SetSuccessHttpCodes(204)
                 .SetAction(h => 
                    () => h.HandleQueryAsync<GetEventsQueryRequest, GetEventsQueryResponse>(new GetEventsQueryRequest())));

            builder.AddGet<GetEventsQueryResponse>(r =>
                r.SetRoute("get-by-id/{id}")
                 .SetSuccessHttpCodes(204)
                 .SetAction<Guid>(h =>
                    (Guid id) => h.HandleQueryAsync<GetEventsQueryRequest, GetEventsQueryResponse>(new GetEventsQueryRequest(id))));

            builder.AddPost<PublishEventCommandResponse>(r =>
                r.SetRoute("publish-event")
                 .SetAction<PublishEventCommandRequest>(h =>
                    (PublishEventCommandRequest req) => h.HandleCommandAsync<PublishEventCommandRequest, PublishEventCommandResponse>(req)));
        }
    }
}
