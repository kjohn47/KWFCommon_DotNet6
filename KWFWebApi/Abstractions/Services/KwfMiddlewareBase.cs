namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;

    public abstract class KwfMiddlewareBase : IKwfMiddleware
    {
        protected readonly RequestDelegate _next;

        public KwfMiddlewareBase(RequestDelegate next)
        {
            _next = next;
        }

        public virtual Task InvokeAsync(HttpContext context)
        {
            return _next(context);
        }
    }
}
