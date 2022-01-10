namespace KWFAuthentication.Abstractions.Constants
{
    public enum PoliciesEnum
    {
        Administrator,
        Anonymous,
        User
    }

    public static class Policies
    {
        public const string Administrator = nameof(PoliciesEnum.Administrator);
        public const string Anonymous = nameof(PoliciesEnum.Anonymous);
        public const string User = nameof(PoliciesEnum.User);

        public static string GetPolicyName(PoliciesEnum policiesEnum)
        {
            switch (policiesEnum)
            {
                case PoliciesEnum.Administrator: return Administrator;
                case PoliciesEnum.Anonymous: return Anonymous;
                case PoliciesEnum.User: return User;
            }

            return string.Empty;
        }

        public static IEnumerable<string> GetRolesList()
        {
            return new[]
            {
                Administrator,
                Anonymous,
                User
            };
        }
    }
}
