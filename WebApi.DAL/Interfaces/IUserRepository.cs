using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Interfaces
{
    public interface IUserRepository : IRepository<IdentityUser>
    {
        Task<IdentityUser> FindByNameAsync(string name);
        Task<IList<string>> GetRolesAsync(IdentityUser user);
        Task<bool> CheckPasswordAsync(IdentityUser user, string password);
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
        Task<IdentityResult> AddToRoleAsync(IdentityUser user, string roleName);
    }
}