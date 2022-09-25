namespace auth.hmac.Interface
{
    public interface IAuthentication
    {
        bool CheckHmacDB(string hmac,int timestamp, string service_name);
    }
}
