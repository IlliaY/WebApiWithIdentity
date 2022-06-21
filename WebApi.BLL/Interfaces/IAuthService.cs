using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using WebApi.BLL.Models;

namespace WebApi.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<Response> LoginAsync(UserLoginModel userLogin);
        Task<Response> RegisterAsync(UserRegisterModel userRegister);
    }
}