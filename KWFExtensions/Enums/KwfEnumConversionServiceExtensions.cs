namespace KWFExtensions.Enums
{
    using Microsoft.Extensions.DependencyInjection;

    public static class KwfEnumConversionServiceExtensions
    {
        public static IServiceCollection AddKwfEnumConverter<TEnum>(this IServiceCollection services)
            where TEnum : struct, Enum
        {
            var converter = new KwfEnumConverter<TEnum>();
            converter.Initialize();
            services.AddSingleton<IKwfEnumConverter<TEnum>>(converter);
            return services;
        }
    }
}
