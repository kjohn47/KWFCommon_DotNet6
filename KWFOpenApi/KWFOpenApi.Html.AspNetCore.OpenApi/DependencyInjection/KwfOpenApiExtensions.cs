namespace KWFOpenApi.Html.AspNetCore.OpenApi.DependencyInjection
{
    using KWFOpenApi.Html.Abstractions;
    using KWFOpenApi.Html.AspNetCore.OpenApi.Provider;
    using KWFOpenApi.Html.DependencyInjection;

    using Microsoft.Extensions.DependencyInjection;

    public static class KwfOpenApiExtensions
    {
        public static IServiceCollection AddKwfSwaggerOpenApiProvider(this IServiceCollection services, string? documentName = null, string? documentUrl = null)
        {
            services.AddSingleton<IKwfApiDocumentProvider>(provider => new OpenApiDocumentProvider(provider, documentName, documentUrl));
            services.AddOpenApiHtmlRenderer();

            return services;
        }
    }
}
