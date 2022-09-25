using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace auth.models.DB
{
    public class TBL_Hmac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string hmac { get; set; }
        public string serviceName { get; set; }
        public int unixtimestamp { get; set; }
        public DateTime dtm_expire { get; set; }
        public bool is_active { get; set; }
    }
}
