
using System.ComponentModel.DataAnnotations;

namespace auth.models.Dto
{
    public class LoginDto
    {
        //[Required]
        public string username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }
    }
}
