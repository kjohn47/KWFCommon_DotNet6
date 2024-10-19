namespace KWFOpenApi.Html.Abstractions
{
    using Microsoft.OpenApi.Models;

    public interface IKwfApiDocumentProvider
    {
        Task<(string documentUrl, OpenApiDocument openApiDocument)> GetOpenApiDocumentAsync();
    }
}
