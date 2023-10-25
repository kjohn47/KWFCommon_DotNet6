namespace Sample.SampleApi.Endpoints
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;
    using Sample.SampleApi.Queries.Enums;

    public class EnumEndpoint : IEndpointConfiguration
    {
        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IKwfEndpointHandler handlers, IConfiguration configuration)
        {
            builder.AddGet<EnumQueryResponse>("get-string")
                 .SetAction(() => handlers.HandleQueryAsync<EnumQueryRequest, EnumQueryResponse>(new EnumQueryRequest { GetStringified = true }));

            builder.AddGet<EnumQueryResponse>("get-enum")
                .SetAction(() => handlers.HandleQueryAsync<EnumQueryRequest, EnumQueryResponse>(new EnumQueryRequest()));

            builder.AddGet<EnumQueryResponse>("get-enum-insensitive")
                .SetAction(() => handlers.HandleQueryAsync<EnumQueryRequest, EnumQueryResponse>(new EnumQueryRequest { GetParsedInsensitive = true }));
        }
    }
}