namespace KWFOpenApi.Html.Abstractions
{
    using KWFOpenApi.Metadata.Models;

    using System.Threading.Tasks;

    public interface IKwfApiDocumentRenderer
    {
        Task<string> GetHtmlForMetadata();

        Task<string> GetHtmlForMetadata(KwfOpenApiMetadata metadata);
    }
}
