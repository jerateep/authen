using auth.models.Data;
using auth.models.DB;
using auth.models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace auth.basic.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _db;
        public UserController(DataContext db)
        {
            _db = db;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public string GetOk()
        {
            return "Ok";
        }

        [Authorize]
        [HttpGet]
        public List<UserDto> GetAllUser()
        {
            return _db.TBL_User.Select(o => new UserDto
            {
                Firstname = o.Firstname,
                Lastname = o.Lastname,
                Username = o.Username
            }).ToList();
        }
    }

}
