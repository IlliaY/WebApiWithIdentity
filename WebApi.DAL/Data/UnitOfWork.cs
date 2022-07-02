using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Interfaces;
using WebApi.DAL.Repositories;

namespace WebApi.DAL.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext context;

        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public UnitOfWork(ApplicationContext context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            UserRepository = new UserRepository(userManager);
            RoleRepository = new RoleRepository(roleManager);
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}