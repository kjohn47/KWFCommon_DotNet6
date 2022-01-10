namespace KWFAuthentication.Abstractions.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public static class ClaimExtensions
    {
        public static ICollection<string> GetClaimValuesList(this IEnumerable<Claim> claims, string type)
        {
            return claims.Where(x => x.Type.Equals(type)).Select(x => x.Value)?.ToList() ?? new List<string>();
        }

        public static string? GetClaimValue(this IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(c => c.Type.Equals(type))?.Value;
        }

        public static Guid GetGuidFromClaim(this IEnumerable<Claim> claims, string claimName)
        {
            try
            {
                var value = claims.GetClaimValue(claimName);

                if (value == null)
                {
                    throw new ArgumentNullException(claimName);
                }

                return Guid.Parse(value);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException)
                {
                    throw;
                }

                throw new Exception($"Invalid guid value for '{claimName}'", ex);
            }
        }

        public static DateTime? GetDateFromClaimUnixTime(this IEnumerable<Claim> claims, string claimName)
        {
            return long.TryParse(claims.GetClaimValue(claimName), out var seconds)
                ? DateTimeOffset.UnixEpoch.DateTime.AddSeconds(seconds)
                : null;
        }
    }
}
