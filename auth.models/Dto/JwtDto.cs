using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth.models.Dto
{
    public class JwtDto
    {
        public string username { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
