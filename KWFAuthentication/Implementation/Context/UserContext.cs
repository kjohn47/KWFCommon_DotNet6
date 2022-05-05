namespace KWFAuthentication.Implementation.Context
{
    using KWFAuthentication.Abstractions.Context;

    using System;
    using System.Collections.Generic;

    internal sealed class UserContext : IUserContext
    {
        public UserContext()
        {
            UserRoles = new List<string>();
            UserId = Guid.Empty;
            SessionId = Guid.Empty;
            UserName = string.Empty;
            Name = string.Empty;
            Surname = string.Empty;
            LanguageCode = string.Empty;
            UserRoles = new List<string>();
            Status = UserStatusEnum.NotLogged;
            TokenExpiration = null;
        }

        public UserContext(
            Guid userId, 
            Guid sessionId,
            string userName,
            string name,
            string surname,
            string languageCode,
            ICollection<string> roles,
            UserStatusEnum status,
            DateTime? tokenExpiration = null)
        {
            UserId = userId;
            SessionId = sessionId;
            UserName = userName;
            Name = name;
            Surname = surname;
            LanguageCode = languageCode;
            UserRoles = roles;
            Status = status;
            TokenExpiration = tokenExpiration;
        }

        public Guid UserId { get; init; }

        public Guid SessionId { get; init; }

        public string UserName { get; init; }

        public string Name { get; init; }

        public string Surname { get; init; }

        public DateTime? TokenExpiration { get; init; }

        public string LanguageCode { get; init; }

        public ICollection<string> UserRoles { get; init; }

        public UserStatusEnum Status { get; init; }
    }
}
