using System.ComponentModel.DataAnnotations;

namespace auth.models.Dto
{
    public class HmacDto
    {
        [Key]
        public string Hmac { get; set; }
        public int UnixTimestamp { get; set; }
    }
}
