using auth.basic.Interface;
using auth.models.Data;
using auth.models.DB;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace auth.basic.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;
        public UserService(DataContext db)
        {
            _db = db;
        }
        
        public  async Task<TBL_User> Authenticate(string _username, string _password)
        {
            TBL_User user = new TBL_User();
            bool result = true;
            user = await _db.TBL_User.FirstOrDefaultAsync(user => user.Username == _username);
            if (user != null) 
            {
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var comoutedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(_password));
                for (int i = 0; i < comoutedHash.Length; i++)
                {
                    if (comoutedHash[i] != user.PasswordHash[i])
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = false;
            }
            if(!result)
            {
                user = new TBL_User();
            }
            return user;
        }
    }
}
