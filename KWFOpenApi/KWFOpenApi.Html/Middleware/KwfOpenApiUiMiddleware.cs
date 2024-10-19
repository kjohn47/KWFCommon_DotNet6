namespace KWFOpenApi.Html.Middleware
{
    using KWFOpenApi.Html.Abstractions;

    using Microsoft.AspNetCore.Http;

    public static class KwfOpenApiUiMiddleware
    {
        private const string DefaultUrl = "kwfopenapi";

        public static async Task InvokeAsync(HttpContext context, Func<Task> next, IKwfApiDocumentRenderer htmlRenderer, string? openApiUIUrl = null)
        {
            var url = $"/{(string.IsNullOrEmpty(openApiUIUrl) ? DefaultUrl : openApiUIUrl)}/index.html";

            await next.Invoke();
        }
    }
}
