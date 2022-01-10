namespace KWFAuthentication.Abstractions.Context
{
    public interface IUserContextAccessor
    {
        IUserContext GetContext();
    }
}
