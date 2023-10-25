namespace Sample.SampleApi.Queries.Enums
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Implementation.CQRS;

    using KWFExtensions.Enums;

    using KWFWebApi.Abstractions.Query;

    using System.Threading;
    using System.Threading.Tasks;

    public class EnumQueryHandler : IQueryHandler<EnumQueryRequest, EnumQueryResponse>
    {
        private readonly IKwfEnumConverter<TestEnum> _enumConverter;

        public EnumQueryHandler(IKwfEnumConverter<TestEnum> enumConverter)
        {
            _enumConverter = enumConverter;
        }

        public Task<ICQRSResult<EnumQueryResponse>> HandleAsync(EnumQueryRequest request, CancellationToken? cancellationToken)
        {
            var response = new EnumQueryResponse();

            if (request.GetStringified)
            {
                response.EnumStringified = _enumConverter.ConvertToString(TestEnum.TestValue);
            }

            if (request.GetParsedInsensitive)
            {
                response.EnumParsed = _enumConverter.ParseFromString(nameof(TestEnum.TestValue).ToLowerInvariant(), true);
            }

            if (!request.GetStringified && !request.GetParsedInsensitive)
            {
                response.EnumParsed = _enumConverter.ParseFromString(nameof(TestEnum.TestValue));
            }

            return Task.FromResult<ICQRSResult<EnumQueryResponse>>(CQRSResult<EnumQueryResponse>.Success(response));
        }
    }
}
