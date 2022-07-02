using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Interfaces
{
    public interface IUserRepository : IRepository<IdentityUser>
    {
        Task<IdentityUser> FindByNameAsync();
        Task<List<string>> GetRolesAsync();
        Task<bool> CheckPasswordAsync();
        Task<IdentityResult> CreateUserAsync();
        Task<IdentityResult> AddToRoleAsync();
    }
}