namespace KWFOpenApi.Html.Swagger.DependencyInjection
{
    using KWFOpenApi.Html.Abstractions;
    using KWFOpenApi.Html.Swagger.Provider;
    using KWFOpenApi.Html.DependencyInjection;

    using Microsoft.Extensions.DependencyInjection;

    public static class KwfOpenApiSwaggerExtensions
    {
        public static IServiceCollection AddKwfSwaggerOpenApiProvider(this IServiceCollection services, string? documentName = null, string? documentUrl = null)
        {
            services.AddSingleton<IKwfApiDocumentProvider>(provider => new SwaggerDocumentProvider(provider, documentName, documentUrl));
            services.AddOpenApiHtmlRenderer();

            return services;
        }
    }
}
