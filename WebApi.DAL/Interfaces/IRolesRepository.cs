using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Interfaces
{
    public interface IRolesRepository : IRepository<IdentityRole>
    {
        Task<bool> RoleExistsAsync();
        Task<IdentityResult> CreateRoleAsync();
    }
}