namespace KWFWebApi.Extensions
{
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Implementation.Services;

    using Microsoft.AspNetCore.Builder;

    public static class KwfApplicationBuilderExtensions
    {
        public static IKwfApplicationBuilder BuildKwfApplication(this WebApplicationBuilder applicationBuilder)
        {
            return KwfApplicationBuilder.BuildKwfApplication(applicationBuilder);
        }
    }
}
