namespace KWFOpenApi.Html.AspNetCore.OpenApi.Provider
{
    using System;
    using System.Threading.Tasks;

    using KWFOpenApi.Html.Abstractions;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    //using Microsoft.AspNetCore.OpenApi;

    public class OpenApiDocumentProvider : IKwfApiDocumentProvider
    {
        private const string DefaultDocumentName = "v1";
        private const string DefaultOpenApiUrl = "openapi/v1.json";

        private readonly IServiceProvider _serviceProvider;
        private readonly string _documentName;
        private readonly string _documentUrl;

        public OpenApiDocumentProvider(IServiceProvider serviceProvider, string? documentName, string? documentUrl)
        {
            _serviceProvider = serviceProvider;
            _documentName = string.IsNullOrEmpty(documentName) ? DefaultDocumentName : documentName;
            _documentUrl = string.IsNullOrEmpty(documentUrl) ? DefaultOpenApiUrl : documentUrl;
        }

        public Task<(string documentUrl, OpenApiDocument openApiDocument)> GetOpenApiDocumentAsync()
        {
            /*
            var targetDocumentService = _serviceProvider.GetRequiredKeyedService<OpenApiDocumentService>(_documentName);
            using var scopedService = _serviceProvider.CreateScope();
            var document = await targetDocumentService.GetOpenApiDocumentAsync(scopedService.ServiceProvider);
            return (_documentUrl, document);
            */
            throw new NotImplementedException();
        }
    }
}
