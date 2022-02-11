namespace Sample.SampleApi.Middleware
{
    using KWFWebApi.Abstractions.Logging;
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Extensions;

    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;

    public class TestMiddleware : KwfMiddlewareBase
    {
        private readonly IKWFLogger<TestMiddleware> _logger;
        public TestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory) : base(next)
        {
            _logger = loggerFactory.CreateKwfLogger<TestMiddleware>();
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Initializing request");
            try
            {
                await _next(context);
                _logger.LogInformation("Processing response");
            }
            catch
            {
                _logger.LogInformation("Processing exception");
                throw;
            }
        }
    }
}
