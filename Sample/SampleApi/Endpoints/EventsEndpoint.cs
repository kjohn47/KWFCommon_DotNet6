namespace Sample.SampleApi.Endpoints
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    using Sample.SampleApi.Commands.Events;
    using Sample.SampleApi.Queries.Events;

    public class EventsEndpoint : IEndpointConfiguration
    {
        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IKwfEndpointHandler handlers, IConfiguration configuration)
        {
            builder.AddGet<GetEventsQueryResponse>("get-all")
                 .SetSuccessHttpCodes(204)
                 .SetAction(() => handlers.HandleQueryAsync<GetEventsQueryRequest, GetEventsQueryResponse>(new GetEventsQueryRequest()));

            builder.AddGet<GetEventsQueryResponse>("get-by-id/{id}")
                .SetSuccessHttpCodes(204)
                .SetAction((Guid id) => handlers.HandleQueryAsync<GetEventsQueryRequest, GetEventsQueryResponse>(new GetEventsQueryRequest(id)));

            builder.AddPost<PublishEventCommandResponse>("publish-event")
                 .SetAction((PublishEventCommandRequest req) => handlers.HandleCommandAsync<PublishEventCommandRequest, PublishEventCommandResponse>(req));
        }
    }
}
