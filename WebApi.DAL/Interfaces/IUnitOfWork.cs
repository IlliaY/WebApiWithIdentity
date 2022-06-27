using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        UserManager<IdentityUser> UserManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        Task SaveAsync();
    }
}