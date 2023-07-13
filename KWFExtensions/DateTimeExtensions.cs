namespace KWFExtensions
{
    using System;

    public static class DateTimeExtensions
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string TimeFormat = "HH:mm:ss";
        public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ssK";

        public static DateTime ToUtcKind(this DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public static DateTime? ToUtcKind(this DateTime? value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Value.ToUtcKind();
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
