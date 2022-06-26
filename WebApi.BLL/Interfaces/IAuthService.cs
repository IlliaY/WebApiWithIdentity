using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using WebApi.BLL.Models;

namespace WebApi.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDTO> LoginAsync(UserLoginModel userLogin);
        Task<MessageDTO> RegisterAsync(UserRegisterModel userRegister);

        Task<MessageDTO> RegisterAdminAsync(UserRegisterModel userRegister);
    }
}