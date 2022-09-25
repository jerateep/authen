using System.Security.Cryptography;
using System.Text;

namespace auth.hmac.Extensions
{
    public class HmacExtensions
    {
        public string HMAC_SHA256(int? _TimeStamp, string _ServiceName, string _SecretKey)
        {
            string strInput = string.Format("{0}{1}", _TimeStamp, _ServiceName);
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] ByteInput = encoding.GetBytes(strInput);
            Byte[] ByteSecretKey = encoding.GetBytes(_SecretKey);
            Byte[] hashBytes;
            using (HMACSHA256 hash = new HMACSHA256(ByteSecretKey))
                hashBytes = hash.ComputeHash(ByteInput);
            string strhmac = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return strhmac;
        }
    }
}
