namespace KWFCommon.Implementation.Exception
{
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text.Json;

    public static class ExceptionHandlerMiddleware
    {
        public static IApplicationBuilder UseExceptionHandlerMidleware(
            this IApplicationBuilder app,
            JsonSerializerOptions serializerOpt,
            bool isDev = false)
        {
            ILogger? logger = app.ApplicationServices.GetService<ILoggerFactory>()?.CreateLogger(nameof(ExceptionHandlerMiddleware));

            app.UseExceptionHandler(a =>
            {
                a.Run(async ctx =>
                {
                    if (ctx.Response.HasStarted)
                    {
                        return;
                    }

                    var error = (IExceptionHandlerFeature?)ctx.Features[typeof(IExceptionHandlerFeature)];
                    if (error?.Error is null)
                    {
                        return;
                    }

                    if (logger is not null && logger.IsEnabled(LogLevel.Error))
                    {
                        logger.LogError(error.Error, "{EXCEPTION_MESSAGE}", error.Error.Message);
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

                        if (error.Error is BadHttpRequestException badHttpRequestException)
                        {
                            ctx.Response.StatusCode = badHttpRequestException.StatusCode;
                            await ctx.Response.WriteAsJsonAsync(
                            new ErrorResult(
                                nameof(BadHttpRequestException),
                                string.IsNullOrEmpty(badHttpRequestException.InnerException?.Message)
                                    ? badHttpRequestException.Message
                                    : string.Concat(badHttpRequestException.Message, "\n", badHttpRequestException.InnerException.Message),
                                (HttpStatusCode)badHttpRequestException.StatusCode,
                                ErrorTypeEnum.Exception),
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
                        case BadHttpRequestException badHttpRequestException:
                            {
                                ctx.Response.StatusCode = badHttpRequestException.StatusCode;
                                await ctx.Response.WriteAsJsonAsync(
                                    new ErrorResult(
                                        nameof(BadHttpRequestException),
                                        $"Application could not respond due to: {badHttpRequestException.Message}",
                                        HttpStatusCode.BadRequest,
                                        ErrorTypeEnum.Exception),
                                    serializerOpt);
                                return;
                            }
                        case HttpRequestException httpEx:
                            {
                                await ctx.Response.WriteAsJsonAsync(
                                    new ErrorResult(
                                        nameof(HttpRequestException),
                                        "Application could not respond: Exception occured",
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
                });
            });

            return app;
        }
    }
}
