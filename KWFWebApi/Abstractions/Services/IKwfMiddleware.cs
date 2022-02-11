namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.AspNetCore.Http;

    public interface IKwfMiddleware
    {
        Task InvokeAsync(HttpContext context);
    }
}
