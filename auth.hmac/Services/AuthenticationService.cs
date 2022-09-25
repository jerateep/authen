using auth.hmac.Interface;
using auth.models.Data;
using auth.models.DB;
using System.Security.Claims;

namespace auth.hmac.Services
{
    public class AuthenticationService : IAuthentication
    {
        private readonly DataContext _db;
        public AuthenticationService(DataContext db)
        {
            _db = db;
        }

        public bool CheckHmacDB(string hmac, int timestamp, string service_name)
        {
            try
            {
                var hmacInfo = _db.TBL_Hmac.FirstOrDefault(o => o.hmac == hmac);
                if (hmacInfo != null)
                {
                    if (!hmacInfo.is_active) return false;
                    if (hmacInfo.dtm_expire < DateTime.Now)
                    {
                        hmacInfo.is_active = false;
                        _db.SaveChanges();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    _db.TBL_Hmac.Add(new TBL_Hmac
                    {
                        dtm_expire = DateTime.Now.AddDays(1),
                        hmac = hmac,
                        serviceName = service_name,
                        unixtimestamp = timestamp,
                        is_active = true
                    });
                    _db.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
