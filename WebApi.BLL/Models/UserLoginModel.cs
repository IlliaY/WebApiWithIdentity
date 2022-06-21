using System.ComponentModel.DataAnnotations;

namespace WebApi.BLL.Models
{
    public class UserLoginModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}