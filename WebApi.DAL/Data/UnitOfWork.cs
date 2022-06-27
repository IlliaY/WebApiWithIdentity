using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext context;

        public UserManager<IdentityUser> UserManager { get; }

        public RoleManager<IdentityRole> RoleManager { get; }

        public UnitOfWork(ApplicationContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}