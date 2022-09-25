using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auth.models.Internal
{
    public class Jwt
    {
        public string secret_key { get; set; } = "Secret_____Key__##___jwt@!!1234567890";
        public int expires { get; set; } = 3;
        public string Issuer { get; set; } = "localhost";
        public string Audience { get; set; } = "localhost.client";

    }
}
