using System.ComponentModel.DataAnnotations;

namespace WebApi.BLL.Models
{
    public class UserLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}