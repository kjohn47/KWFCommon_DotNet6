namespace KWFOpenApi.Html.Document
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    //using Microsoft.AspNetCore.OpenApi;

    //using Swashbuckle.AspNetCore.Swagger;

    using KWFOpenApi.Metadata.Models;
    using KWFOpenApi.Html.Abstractions;
    using KWFOpenApi.Metadata;

    public class KwfApiDocumentRender : IKwfApiDocumentRenderer
    {
        /*
        private const string DefaultDocumentName = "v1";
        private const string DefaultSwaggerUrl = "swagger/v1/swagger.json";
        private const string DefaultOpenApiUrl = "openapi/v1.json";
        */

        private readonly IServiceProvider _serviceProvider;
        private readonly IKwfApiDocumentProvider _kwfApiDocumentProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<KwfApiDocumentRender> _logger;

        private string? _renderedPage;

        public KwfApiDocumentRender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _kwfApiDocumentProvider = serviceProvider.GetRequiredService<IKwfApiDocumentProvider>();
            _loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger<KwfApiDocumentRender>();
        }

        /*
        public async Task<string> GetHtmlSwagger(string swaggerUrl = DefaultSwaggerUrl, string documentName = DefaultDocumentName)
        {
            //for swagger
            var swaggerProvider = _serviceProvider.GetRequiredService<IAsyncSwaggerProvider>();
            var document = await swaggerProvider.GetSwaggerAsync(documentName);
            var metadata = document.GenerateMetadata(swaggerUrl);
            return await GetHtmlForMetadata(metadata);
        }

        public async Task<string> GetHtmlOpenApi(string openApiUrl = DefaultOpenApiUrl, string documentName = DefaultDocumentName)
        {
            throw new NotImplementedException();
            /*
             var targetDocumentService = _serviceProvider.GetRequiredKeyedService<OpenApiDocumentService>(documentName);
             using var scopedService = _serviceProvider.CreateScope();
             var document = await targetDocumentService.GetOpenApiDocumentAsync(scopedService.ServiceProvider);
             var metadata = document.GenerateMetadata(openApiUrl);

            return await GetHtmlForMetadata(metadata);
            
        }
        */

        public async Task<string> GetHtmlForMetadata()
        {
            if (!string.IsNullOrEmpty(_renderedPage))
            {
                return _renderedPage;
            }

            var (documentUrl, openApiDocument) = await _kwfApiDocumentProvider.GetOpenApiDocumentAsync();
            
            return await GetHtmlForMetadata(openApiDocument.GenerateMetadata(documentUrl));
        }

        public async Task<string> GetHtmlForMetadata(KwfOpenApiMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            _renderedPage = await RenderDocument(metadata);

            return _renderedPage;
        }

        private async Task<string> RenderDocument(KwfOpenApiMetadata metadata)
        {
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);

            try
            {
                var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var dictionary = new Dictionary<string, object?>
                    {
                        { "Metadata", metadata }
                    };

                    var parameters = ParameterView.FromDictionary(dictionary);
                    var output = await htmlRenderer.RenderComponentAsync<KwfApiDocumentation>(parameters);

                    return output.ToHtmlString();
                });

                return html;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured during render of open api document");
                return string.Empty;
            }
        }
    }
}
