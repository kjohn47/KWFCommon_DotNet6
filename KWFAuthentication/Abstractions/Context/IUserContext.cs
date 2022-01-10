namespace KWFAuthentication.Abstractions.Context
{
    using System;
    using System.Collections.Generic;

    public interface IUserContext
    {
        Guid UserId { get; }
        Guid SessionId { get; }
        string UserName { get; }
        string Name { get; }
        string Surname { get; }
        DateTime? TokenExpiration { get; }
        string LanguageCode { get; }
        ICollection<string> UserRoles { get; }
        UserStatusEnum Status { get; }
    }
}
