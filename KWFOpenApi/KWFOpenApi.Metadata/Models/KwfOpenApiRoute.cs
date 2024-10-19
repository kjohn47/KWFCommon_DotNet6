namespace KWFOpenApi.Metadata.Models
{
    using System.Net;

    public class KwfOpenApiRoute
    {
        public required string Route { get; set; }
        public required string Method { get; set; }
        public required string Operation { get; set; }
        public required string Summary { get; set; }
        public List<KwfParam>? RouteParams { get; set; }
        public List<KwfParam>? QueryParams { get; set; }
        public List<KwfParam>? HeaderParams { get; set; }
        public Dictionary<KwfRequestBodyType, KwfContentBody>? RequestBodies { get; set; }
        public Dictionary<HttpStatusCode, Dictionary<KwfRequestBodyType, KwfContentBody>>? ResponseSamples { get; set; }
    }
}
