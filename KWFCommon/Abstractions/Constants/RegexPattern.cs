namespace KWFCommon.Abstractions.Constants
{
    public static class RegexPattern
    {
        public const string UserName = @"^[A-Za-z][a-zA-Z0-9._-]*[a-zA-Z0-9]$";
        public const string OnlyLetter = @"^[A-Za-z]*$";
        public const string OnlyNumber = @"^[0-9]*$";
        public const string NoSymbols = @"^[a-zA-Z0-9]*$";
        public const string Email = @"[_A-Za-z0-9\-\+]+(\.[_A-Za-z0-9\-]+)*@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*(\.[A-Za-z]{2,})";
    }
}
