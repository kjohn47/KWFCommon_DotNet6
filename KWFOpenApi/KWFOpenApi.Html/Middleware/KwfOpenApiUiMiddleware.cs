namespace KWFOpenApi.Html.Middleware
{
    using KWFOpenApi.Html.Abstractions;

    using Microsoft.AspNetCore.Http;

    using System;
    using System.Net.Mime;

    public class KwfOpenApiUiMiddleware
    {
        private const string DefaultUrl = "kwfopenapi";
        private readonly IKwfApiDocumentRenderer _kwfDocumentRenderer;
        private readonly string _url_1;
        private readonly string _url_2;
        private readonly string _urlIndex;
        private readonly string _urlJs;
        private readonly string _urlCss;

        public KwfOpenApiUiMiddleware(IKwfApiDocumentRenderer htmlRenderer, string? openApiUIUrl = null)
        {
            _url_1 = $"/{(string.IsNullOrEmpty(openApiUIUrl) ? DefaultUrl : openApiUIUrl)}";
            _url_2 = $"{_url_1}/";
            _urlIndex = $"{_url_1}/index.html";
            _urlJs = $"{_url_1}/kwfopenapi.js";
            _urlCss = $"{_url_1}/kwfopenapi.css";

            _kwfDocumentRenderer = htmlRenderer;
        }

        public async Task InvokeAsync(HttpContext context, Func<Task> next)
        {
            if (context.Request?.Path.Value != null && context.Request.Path.StartsWithSegments(_url_1, StringComparison.InvariantCultureIgnoreCase))
            {
                if (context.Request.Path.Equals(_urlJs, StringComparison.InvariantCultureIgnoreCase))
                {
                    var jsFile = _kwfDocumentRenderer.GetKwfOpenApiJs();
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = MediaTypeNames.Text.JavaScript;
                    await context.Response.WriteAsync(jsFile);

                    return;
                }

                if (context.Request.Path.Equals(_urlCss, StringComparison.InvariantCultureIgnoreCase))
                {
                    var cssFile = _kwfDocumentRenderer.GetKwfOpenApiCss();
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = MediaTypeNames.Text.Css;
                    await context.Response.WriteAsync(cssFile);

                    return;
                }

                if (context.Request.Path.Equals(_url_1, StringComparison.InvariantCultureIgnoreCase) || 
                    context.Request.Path.Equals(_url_2, StringComparison.InvariantCultureIgnoreCase) ||
                    context.Request.Path.Equals(_urlIndex, StringComparison.InvariantCultureIgnoreCase))
                {
                    var htmlPage = await _kwfDocumentRenderer.GetHtmlForMetadata();
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = MediaTypeNames.Text.Html;
                    await context.Response.WriteAsync(htmlPage);

                    return;
                }
            }

            await next.Invoke();
        }
    }
}
