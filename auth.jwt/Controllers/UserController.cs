using auth.models.Data;
using auth.models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auth.jwt.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _db;
        public UserController(DataContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<UserDto>> GetAllUser()
        {
            return await _db.TBL_User.Select(o => new UserDto
            {
                Username = o.Username,
                Firstname = o.Firstname,
                Lastname = o.Lastname
            }).ToListAsync();
        }

        [HttpGet]
        public string GetOk()
        {
            return "Ok";
        }
    }
}
