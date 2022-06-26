using System.ComponentModel.DataAnnotations;

namespace WebApi.BLL.Models
{
    public class UserRegisterModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}