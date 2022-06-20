using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using WebApi.BLL.Models;

namespace WebApi.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<JwtSecurityToken> LoginAsync(UserLoginModel userLogin);
        Task<string> Register(UserRegisterModel userRegister);
    }
}