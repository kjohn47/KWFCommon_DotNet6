namespace KWFAuthentication.Abstractions.Context
{
    public static class UserStatusExtensions
    {
        public static IEnumerable<string> GetStatusList()
        {
            return new[]
            {
                nameof(UserStatusEnum.Inactive),
                nameof(UserStatusEnum.Active),
                nameof(UserStatusEnum.Blocked),
                nameof(UserStatusEnum.Disabled)
            };
        }

        public static string GetUserStatusString(this UserStatusEnum userStatus)
        {
            return userStatus switch
            {
                UserStatusEnum.NotLogged => nameof(UserStatusEnum.NotLogged),
                UserStatusEnum.Inactive => nameof(UserStatusEnum.Inactive),
                UserStatusEnum.Active => nameof(UserStatusEnum.Active),
                UserStatusEnum.Blocked => nameof(UserStatusEnum.Blocked),
                UserStatusEnum.Disabled => nameof(UserStatusEnum.Disabled),
                _ => nameof(UserStatusEnum.NotLogged),
            };
        }

        public static UserStatusEnum GetUserStatusValue(this string userStatus)
        {
            return userStatus switch
            {
                nameof(UserStatusEnum.NotLogged) => UserStatusEnum.NotLogged,
                nameof(UserStatusEnum.Inactive) => UserStatusEnum.Inactive,
                nameof(UserStatusEnum.Active) => UserStatusEnum.Active,
                nameof(UserStatusEnum.Blocked) => UserStatusEnum.Blocked,
                nameof(UserStatusEnum.Disabled) => UserStatusEnum.Disabled,
                _ => UserStatusEnum.NotLogged,
            };
        }
    }
}
