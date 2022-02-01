namespace KWFCommon.Implementation.ServiceConfiguration
{
    using System;
    using System.Globalization;

    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;

    public static class LocalizationConfigurator
    {
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, LocalizationConfiguration? configuration)
        {
            if (string.IsNullOrEmpty(configuration?.LocalizationCode))
            {
                throw new ArgumentNullException(
                    "LocalizationConfiguration:LocalizationCode",
                    "Missing Localization Configuration or Localization Code on App Configuration");
            }

            var cultureInfo = new CultureInfo(configuration.LocalizationCode);
            if (!string.IsNullOrEmpty(configuration.CurrencyLocalizationCode))
            {
                var currencyLocalization = new CultureInfo(configuration.CurrencyLocalizationCode);
                cultureInfo.NumberFormat.CurrencySymbol = currencyLocalization.NumberFormat.CurrencySymbol;
            }

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            return app;
        }
    }
}
