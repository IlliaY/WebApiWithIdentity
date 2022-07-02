using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Interfaces
{
    public interface IRoleRepository : IRepository<IdentityRole>
    {
        Task<bool> RoleExistsAsync();
        Task<IdentityResult> CreateRoleAsync();
    }
}