namespace KWFExtensions.Enums
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KwfEnumConversionServiceExtensions
    {
        public static IServiceCollection AddKwfEnumConverter<TEnum>(this IServiceCollection services)
            where TEnum : struct, Enum
        {
            var converter = new KwfEnumConverter<TEnum>();
            converter.Initialize();
            services.TryAddSingleton<IKwfEnumConverter<TEnum>>(converter);
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            return services;
        }
        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4, TEnum5>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
            where TEnum5 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            services.AddKwfEnumConverter<TEnum5>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4, TEnum5, TEnum6>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
            where TEnum5 : struct, Enum
            where TEnum6 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            services.AddKwfEnumConverter<TEnum5>();
            services.AddKwfEnumConverter<TEnum6>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4, TEnum5, TEnum6, TEnum7>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
            where TEnum5 : struct, Enum
            where TEnum6 : struct, Enum
            where TEnum7 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            services.AddKwfEnumConverter<TEnum5>();
            services.AddKwfEnumConverter<TEnum6>();
            services.AddKwfEnumConverter<TEnum7>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4, TEnum5, TEnum6, TEnum7, TEnum8>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
            where TEnum5 : struct, Enum
            where TEnum6 : struct, Enum
            where TEnum7 : struct, Enum
            where TEnum8 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            services.AddKwfEnumConverter<TEnum5>();
            services.AddKwfEnumConverter<TEnum6>();
            services.AddKwfEnumConverter<TEnum7>();
            services.AddKwfEnumConverter<TEnum8>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4, TEnum5, TEnum6, TEnum7, TEnum8, TEnum9>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
            where TEnum5 : struct, Enum
            where TEnum6 : struct, Enum
            where TEnum7 : struct, Enum
            where TEnum8 : struct, Enum
            where TEnum9 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            services.AddKwfEnumConverter<TEnum5>();
            services.AddKwfEnumConverter<TEnum6>();
            services.AddKwfEnumConverter<TEnum7>();
            services.AddKwfEnumConverter<TEnum8>();
            services.AddKwfEnumConverter<TEnum9>();
            return services;
        }

        public static IServiceCollection AddKwfEnumConverterForMultiple<TEnum1, TEnum2, TEnum3, TEnum4, TEnum5, TEnum6, TEnum7, TEnum8, TEnum9, TEnum10>(this IServiceCollection services)
            where TEnum1 : struct, Enum
            where TEnum2 : struct, Enum
            where TEnum3 : struct, Enum
            where TEnum4 : struct, Enum
            where TEnum5 : struct, Enum
            where TEnum6 : struct, Enum
            where TEnum7 : struct, Enum
            where TEnum8 : struct, Enum
            where TEnum9 : struct, Enum
            where TEnum10 : struct, Enum
        {
            services.AddKwfEnumConverter<TEnum1>();
            services.AddKwfEnumConverter<TEnum2>();
            services.AddKwfEnumConverter<TEnum3>();
            services.AddKwfEnumConverter<TEnum4>();
            services.AddKwfEnumConverter<TEnum5>();
            services.AddKwfEnumConverter<TEnum6>();
            services.AddKwfEnumConverter<TEnum7>();
            services.AddKwfEnumConverter<TEnum8>();
            services.AddKwfEnumConverter<TEnum9>();
            services.AddKwfEnumConverter<TEnum10>();
            return services;
        }
    }
}
