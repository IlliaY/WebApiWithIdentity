using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.BLL.Interfaces
{
    public interface IJwtCreationService
    {
        JwtSecurityToken GenerateToken(List<Claim> claims);
    }
}