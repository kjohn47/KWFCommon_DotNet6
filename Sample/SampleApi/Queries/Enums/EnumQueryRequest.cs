namespace Sample.SampleApi.Queries.Enums
{
    using KWFWebApi.Abstractions.Query;

    public class EnumQueryRequest : IQueryRequest
    {
        public bool GetStringified { get; set; }
        public bool GetParsedInsensitive { get; set; }
    }
}
