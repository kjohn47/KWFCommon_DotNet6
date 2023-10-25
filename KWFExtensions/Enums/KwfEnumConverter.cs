namespace KWFExtensions.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class KwfEnumConverter<TEnum> : IKwfEnumConverter<TEnum>, IKwfEnumConverterInitializer
        where TEnum : struct, Enum
    {
        private IDictionary<string, TEnum>? _stringEnumDictionary;
        private IDictionary<string, TEnum>? _stringEnumDictionaryIgnoreCase;
        private IDictionary<TEnum, string>? _enumStringDictionary;

        public void Initialize()
        {
            var stringEnumDictionary = new Dictionary<string, TEnum>();
            var stringEnumDictionaryIgnoreCase = new Dictionary<string, TEnum>();
            var enumStringDictionary = new Dictionary<TEnum, string>();

            var enumValues = Enum.GetValues<TEnum>();

            if (enumValues is null || !enumValues.Any())
            {
                throw new ArgumentNullException(nameof(enumValues), "Enum cannot be null or empty");
            }

            foreach (var enumValue in enumValues)
            {
                var stringValue = enumValue.ToString();
                stringEnumDictionary.Add(stringValue, enumValue);
                stringEnumDictionaryIgnoreCase.Add(stringValue.ToUpperInvariant(), enumValue);
                enumStringDictionary.Add(enumValue, stringValue);
            }

            _stringEnumDictionary = stringEnumDictionary;
            _enumStringDictionary = enumStringDictionary;
            _stringEnumDictionaryIgnoreCase = stringEnumDictionaryIgnoreCase;
        }

        public string ConvertToString(TEnum value)
        {
            if (_enumStringDictionary is null)
            {
                Initialize();
            }

            return _enumStringDictionary!.TryGetValue(value, out var enumValue) ? enumValue : throw new NotImplementedException($"{value} not implemented in enum {nameof(TEnum)}");
        }

        public string? ConvertToString(TEnum? value)
        {
            if (value is null)
            {
                return null;
            }
            
            return ConvertToString(value.Value);
        }

        public TEnum ParseFromString(string value, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null");
            }

            if (ignoreCase)
            {
                return ParseFromStringIgnoreCase(value);
            }

            if (_stringEnumDictionary is null)
            {
                Initialize();
            }

            return _stringEnumDictionary!.TryGetValue(value, out var enumValue) ? enumValue : throw new NotImplementedException($"{value} not implemented in enum {nameof(TEnum)}");
        }

        private TEnum ParseFromStringIgnoreCase(string value)
        {
            if (_stringEnumDictionaryIgnoreCase is null)
            {
                Initialize();
            }

            return _stringEnumDictionaryIgnoreCase!.TryGetValue(value.ToUpperInvariant(), out var enumValue) ? enumValue : throw new NotImplementedException($"{value} not implemented in enum {nameof(TEnum)}");
        }
    }
}
