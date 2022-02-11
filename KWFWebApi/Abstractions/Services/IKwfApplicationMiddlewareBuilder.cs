namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationMiddlewareBuilder : IKwfApplicationServicesBuilder
    {
        IKwfApplicationMiddlewareBuilder AddMiddleware<T>() where T : KwfMiddlewareBase;
    }
}
