
using auth.hmac.Interface;
using auth.models.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace auth.hmac.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private string SecretKey = "@uthAp1_test!!";
        private readonly IAuthentication _authenService;
        public UserController(IAuthentication authenService)
        {
            _authenService = authenService;
        }

        [HttpGet]
        public string GetName()
        {
            var hmacHeader = Request.Headers["hmac"].FirstOrDefault()?.Split(" ").Last();
            var timestampHeader = Request.Headers["timestamp"].FirstOrDefault()?.Split(" ").Last();
            var ServiceName = Request.Headers["sercice_name"].FirstOrDefault()?.Split(" ").Last();
            int timestamp = Convert.ToInt32(timestampHeader);
            string strHmac = new Extensions.HmacExtensions().HMAC_SHA256(timestamp, ServiceName, SecretKey);         
            if (strHmac != hmacHeader) return "hmac invalid";
            bool IsActive = _authenService.CheckHmacDB(strHmac, timestamp, ServiceName);
            if (!IsActive) return "token expried.";
            return "jerateep saelee";
            
        }

        [HttpGet]
        public string GetNameAuth()
        {
            return "jerateep saelee";
        }

        [AllowAnonymous]
        [HttpGet]
        public HmacDto GetHmac()
        {
            string ServiceName = "HRM";
            Int32 UnixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string strHmac = new Extensions.HmacExtensions().HMAC_SHA256(UnixTimestamp, ServiceName, SecretKey);
            return new HmacDto
            {
                UnixTimestamp = UnixTimestamp,
                Hmac = strHmac
            };
        }
    }
}
