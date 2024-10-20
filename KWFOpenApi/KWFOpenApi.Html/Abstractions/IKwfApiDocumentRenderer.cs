namespace KWFOpenApi.Html.Abstractions
{
    using KWFOpenApi.Metadata.Models;

    using System.Threading.Tasks;

    public interface IKwfApiDocumentRenderer
    {
        string GetKwfOpenApiCss();

        string GetKwfOpenApiJs();

        Task<string> GetHtmlForMetadata();

        Task<string> GetHtmlForMetadata(KwfOpenApiMetadata metadata);
    }
}
