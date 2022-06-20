using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext context;

        public UnitOfWork(ApplicationContext context)
        {
            this.context = context;
        }
        public UserManager<IdentityUser> UserManager { get; set; }

        public RoleManager<IdentityRole> RoleManager { get; set; }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}