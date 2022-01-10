namespace KWFCommon.Implementation.Crypt
{
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;

    using System;
    using System.Security.Cryptography;

    public static class PasswordManager
    {
        public static string GetSaltedPassword(this string password, string username)
        {
            var salt = GeneratePasswordSalt();
            var saltedPw = password.GetHashFromPassword().SaltPassword(salt);
            return string.Concat(saltedPw.GetBase64FromBytes(), ".", salt.XorPasswordSalt(username).GetBase64FromBytes());
        }

        public static bool IsPasswordValid(this string password, string username, string saltedHashedPassword)
        {
            var hashAndSalt = saltedHashedPassword.Split('.');
            var cypherPw = Convert.FromBase64String(hashAndSalt[0]);
            var salt = Convert.FromBase64String(hashAndSalt[1]).XorPasswordSalt(username);
            var cypherPw2 = password.GetHashFromPassword().SaltPassword(salt);

            if (cypherPw == null || cypherPw2 == null || cypherPw.Length != cypherPw2.Length)
            {
                return false;
            }

            var areSame = true;

            for (var i = 0; i < cypherPw.Length; i++)
            {
                areSame &= (cypherPw[i] == cypherPw2[i]);
            }

            return areSame;
        }

        private static string GetHashFromPassword(this string password)
        {
            return string.Concat(
                    password.GetBase64Sha256HashFromString(), 
                    password.ToUpperInvariant().GetBase64Sha256HashFromString())
                .GetBase64Sha512HashFromString();
        }

        private static byte[] GeneratePasswordSalt()
        {
            var salt = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        private static byte[] XorPasswordSalt(this byte[] salt, string username)
        {
            var usernameHash = new byte[32];
            var xorSalt = new byte[32];

            using (var hash = SHA256.Create())
            {
                usernameHash = hash.ComputeHash(string.Concat(username).GetBytesFromString());
            }

            for (int i = 0; i < xorSalt.Length; i++)
            {
                xorSalt[i] = (byte)(salt[i] ^ usernameHash[i]);
            }

            return xorSalt;
        }

        private static byte[] SaltPassword(this string hashedPassword, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(hashedPassword, salt, KeyDerivationPrf.HMACSHA512, 30, 64);
        }
    }
}
