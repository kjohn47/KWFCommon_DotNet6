namespace KWFExtensions.Enums
{
    using System;

    public interface IKwfEnumConverter<TEnum>
        where TEnum : struct, Enum
    {
        string ConvertToString(TEnum value);

        string? ConvertToString(TEnum? value);

        TEnum ParseFromString(string value, bool ignoreCase = false);
    }
}
