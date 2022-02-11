namespace KWFCommon.Implementation.Exception
{
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;

    using System.Net;
    using System.Text.Json;

    public static class ExceptionHandlerMiddleware
    {
        public static IApplicationBuilder UseExceptionHandlerMidleware(
            this IApplicationBuilder app,
            JsonSerializerOptions serializerOpt,
            bool isDev = false)
        {
            app.UseExceptionHandler(a => a.Run(async ctx =>
            {
                if (ctx.Response.HasStarted)
                {
                    return;
                }

                var error = (IExceptionHandlerFeature?)ctx.Features[typeof(IExceptionHandlerFeature)];
                if (error is null)
                {
                    return;
                }

                if (isDev)
                {
                    if (error.Error is KWFHandledException handledException)
                    {
                        ctx.Response.StatusCode = (int)handledException.HttpStatusCode;

                        await ctx.Response.WriteAsJsonAsync(
                            new ErrorResult(
                                handledException.ErrorCode,
                                string.IsNullOrEmpty(handledException.InnerException?.Message)
                                    ? handledException.Message
                                    : string.Concat(handledException.Message, "\n", handledException.InnerException.Message),
                                handledException.HttpStatusCode,
                                handledException.ErrorType),
                            serializerOpt);
                        return;
                    }

                    await ctx.Response.WriteAsJsonAsync(
                        new ErrorResult(
                            error.Error is HttpRequestException 
                                ? nameof(HttpRequestException)
                                : nameof(ErrorTypeEnum.Exception),
                            string.IsNullOrEmpty(error.Error.InnerException?.Message) 
                                ? error.Error.Message
                                : string.Concat(error.Error.Message, "\n", error.Error.InnerException.Message), 
                            (HttpStatusCode)ctx.Response.StatusCode, 
                            ErrorTypeEnum.Exception),
                        serializerOpt);
                    return;
                }

                switch (error.Error)
                {
                    case HttpRequestException httpEx:
                        {
                            await ctx.Response.WriteAsJsonAsync(
                                new ErrorResult(
                                    nameof(HttpRequestException),
                                    "Application could not respond.",
                                    ErrorTypeEnum.Exception),
                                serializerOpt);
                            return;
                        }
                    case KWFHandledException handledEx:
                        {
                            ctx.Response.StatusCode = (int)handledEx.HttpStatusCode;

                            await ctx.Response.WriteAsJsonAsync(
                                new ErrorResult(
                                    handledEx.ErrorCode,
                                    handledEx.Message,
                                    handledEx.HttpStatusCode,
                                    handledEx.ErrorType),
                                serializerOpt);
                            return;
                        }
                    default:
                        {
                            await ctx.Response.WriteAsJsonAsync(
                                new ErrorResult(
                                    nameof(ErrorTypeEnum.Unknown),
                                    "Unknown error occured",
                                    ErrorTypeEnum.Exception), 
                                serializerOpt);
                            return;
                        }
                }
            }));

            return app;
        }
    }
}
