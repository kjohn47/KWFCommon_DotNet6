namespace KWFOpenApi.Html.Middleware
{
    using KWFOpenApi.Html.Abstractions;

    using Microsoft.AspNetCore.Http;
    using System.Net.Mime;

    public static class KwfOpenApiUiMiddleware
    {
        private const string DefaultUrl = "kwfopenapi";

        public static async Task InvokeAsync(HttpContext context, Func<Task> next, IKwfApiDocumentRenderer htmlRenderer, string? openApiUIUrl = null)
        {
            var url = $"/{(string.IsNullOrEmpty(openApiUIUrl) ? DefaultUrl : openApiUIUrl)}/index.html";

            if (context.Request.Path.Equals(url, StringComparison.InvariantCultureIgnoreCase))
            {
                var htmlPage = await htmlRenderer.GetHtmlForMetadata();
                context.Response.StatusCode = 200;
                context.Response.ContentType = MediaTypeNames.Text.Html;
                await context.Response.WriteAsync(htmlPage);

                return;
            }

            await next.Invoke();
        }
    }
}
