namespace Sample.SampleApi.Queries.Enums
{
    using KWFWebApi.Abstractions.Query;

    public class EnumQueryResponse : IQueryResponse
    {
        public string? EnumStringified { get; set; }
        public TestEnum? EnumParsed { get; set; }
    }
}
