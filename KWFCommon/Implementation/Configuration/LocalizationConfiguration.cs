namespace KWFCommon.Implementation.Configuration
{
    public sealed class LocalizationConfiguration
    {
        public string? DefaultLocalizationCode { get; set; }
        public IEnumerable<string>? SupportedLocalizationCodes { get; set; }
        public string? CurrencyLocalizationCode { get; set; }
    }
}
