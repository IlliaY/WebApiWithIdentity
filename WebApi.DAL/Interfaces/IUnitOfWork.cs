using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }

        public IRoleRepository RoleRepository { get; }
        Task SaveAsync();
    }
}