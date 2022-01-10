namespace KWFCommon.Implementation.Crypt
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class ShaExtensions
    {
        public static string GetBase64Sha1HashFromString(this string value)
        {
            return value.GetBytesFromString().GetBase64Sha1HashFromBytes();
        }

        public static string GetBase64Sha256HashFromString(this string value)
        {
            return value.GetBytesFromString().GetBase64Sha256HashFromBytes();
        }

        public static string GetBase64Sha512HashFromString(this string value)
        {
            return value.GetBytesFromString().GetBase64Sha512HashFromBytes();
        }

        public static string GetBase64Sha1HashFromBytes(this byte[] bytes)
        {
            using (var hash = SHA1.Create())
            {
                return hash.ComputeHash(bytes).GetBase64FromBytes();
            }
        }

        public static string GetBase64Sha256HashFromBytes(this byte[] bytes)
        {
            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(bytes).GetBase64FromBytes();
            }
        }

        public static string GetBase64Sha512HashFromBytes(this byte[] bytes)
        {
            using (var hash = SHA512.Create())
            {
                return hash.ComputeHash(bytes).GetBase64FromBytes();
            }
        }

        public static byte[] GetBytesFromString(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string GetBase64FromBytes(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
