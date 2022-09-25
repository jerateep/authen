using auth.models.DB;

namespace auth.basic.Interface
{
    public interface IUserService
    {
        Task<TBL_User> Authenticate(string _username, string _password);
    }
}
