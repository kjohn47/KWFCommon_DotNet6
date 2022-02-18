namespace KWFCommon.Implementation.ServiceConfiguration
{
    using System;
    using System.Globalization;

    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Localization;

    public static class LocalizationConfigurator
    {
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app, LocalizationConfiguration? configuration)
        {
            bool hasDefaultCulture = !string.IsNullOrEmpty(configuration?.DefaultLocalizationCode);
            bool hasSupportedCultures = configuration?.SupportedLocalizationCodes != null && configuration.SupportedLocalizationCodes.Any();

            if (!hasDefaultCulture && !hasSupportedCultures)
            {
                throw new ArgumentNullException(
                    "LocalizationConfiguration:[DefaultLocalizationCode][SupportedLocalizationCode]",
                    "Missing Localization Configuration or Default/Supported Localization Codes on App Configuration");
            }

            var defaultCultureInfo = hasDefaultCulture
                                    ? new CultureInfo(configuration!.DefaultLocalizationCode!)
                                    : new CultureInfo(configuration!.SupportedLocalizationCodes!.First());

            CultureInfo? currencyCulture = null;
            if (!string.IsNullOrEmpty(configuration?.CurrencyLocalizationCode))
            {
                currencyCulture = new CultureInfo(configuration.CurrencyLocalizationCode!);
                defaultCultureInfo.MapCurrencyValues(currencyCulture);
            }

            var supportedCultures = new List<CultureInfo>
            {
                defaultCultureInfo
            };

            if (hasSupportedCultures)
            {
                foreach (var cultureCode in configuration!.SupportedLocalizationCodes!)
                {
                    if (supportedCultures.Any(s => s.Name.Equals(cultureCode)))
                    {
                        continue;
                    }
                    var culture = new CultureInfo(cultureCode);

                    if (currencyCulture is not null)
                    {
                        culture.MapCurrencyValues(currencyCulture);
                    }

                    supportedCultures.Add(culture);
                }
            }

            CultureInfo.DefaultThreadCurrentCulture = defaultCultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCultureInfo;

            app.UseRequestLocalization(opt =>
            {
                opt.DefaultRequestCulture = new RequestCulture(defaultCultureInfo);
                opt.SupportedCultures = supportedCultures;
                opt.SupportedUICultures = supportedCultures;
                opt.RequestCultureProviders = new[] { new AcceptLanguageHeaderRequestCultureProvider() };
            });

            return app;
        }

        private static CultureInfo MapCurrencyValues(this CultureInfo cultureInfo, CultureInfo currencyCulture)
        {
            if (cultureInfo.Name.Equals(currencyCulture.Name))
            {
                return cultureInfo;
            }

            cultureInfo.NumberFormat.CurrencySymbol = currencyCulture.NumberFormat.CurrencySymbol;
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = currencyCulture.NumberFormat.CurrencyDecimalSeparator;
            cultureInfo.NumberFormat.CurrencyDecimalDigits = currencyCulture.NumberFormat.CurrencyDecimalDigits;
            cultureInfo.NumberFormat.CurrencyGroupSeparator = currencyCulture.NumberFormat.CurrencyGroupSeparator;
            cultureInfo.NumberFormat.CurrencyGroupSizes = currencyCulture.NumberFormat.CurrencyGroupSizes;
            cultureInfo.NumberFormat.CurrencyNegativePattern = currencyCulture.NumberFormat.CurrencyNegativePattern;
            cultureInfo.NumberFormat.CurrencyPositivePattern = currencyCulture.NumberFormat.CurrencyPositivePattern;

            return cultureInfo;
        }
    }
}
