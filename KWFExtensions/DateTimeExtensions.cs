namespace KWFExtensions
{
    using System;

    public static class DateTimeExtensions
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string TimeFormat = "HH:mm:ss";
        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssK";
        public const string TimeStampFormat = "yyyy-MM-ddTHH:mm:ss.fff";
        public const string DateWithoutSeparatorFormat = "yyyyMMdd";

        public static DateTime ToUtc(this DateTime value)
        {
            return value.Kind == DateTimeKind.Unspecified
                   ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
                   : value.ToUniversalTime();
        }

        public static DateTime? ToUtc(this DateTime? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value.ToUtc();
        }

        public static string GetUtcDateTimeString(this DateTime value)
        {
            return value.ToString(DateTimeFormat);
        }

        public static string? GetDateOnlyString(this DateTime? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value.GetUtcDateTimeString();
        }

        public static string GetUtcDateTimeString(this DateTimeOffset value)
        {
            return value.ToString(DateTimeFormat);
        }

        public static string? GetDateOnlyString(this DateTimeOffset? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value.GetUtcDateTimeString();
        }

        public static string GetDateOnlyString(this DateOnly value)
        {
            return value.ToString(DateFormat);
        }

        public static string? GetDateOnlyString(this DateOnly? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value.GetDateOnlyString();
        }

        public static string GetTimeOnlyString(this TimeOnly value)
        {
            return value.ToString(TimeFormat);
        }

        public static string? GetTimeOnlyString(this TimeOnly? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value.GetTimeOnlyString();
        }
    }
}
