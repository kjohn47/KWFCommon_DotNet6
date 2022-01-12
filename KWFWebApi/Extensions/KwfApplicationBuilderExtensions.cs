namespace KWFWebApi.Extensions
{
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Implementation.Services;

    using Microsoft.AspNetCore.Builder;

    public static class KwfApplicationBuilderExtensions
    {
        public static IKwfApplicationBuilder BuildKwfApplication(this WebApplicationBuilder applicationBuilder)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                null,
                null,
                null);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            string? customAppConfigurationKey)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                null,
                null);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                null);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(this WebApplicationBuilder applicationBuilder, bool enableAuthentication)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                null,
                null,
                null,
                enableAuthentication);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                null,
                null,
                enableAuthentication);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                null,
                enableAuthentication);
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            this WebApplicationBuilder applicationBuilder,
            bool enableAuthentication,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey)
        {
            return KwfApplicationBuilder.BuildKwfApplication(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication);
        }
    }
}
