using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Data;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> userManager;

        public UserRepository(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task AddAsync(IdentityUser user)
        {
            var password = user.PasswordHash;
            await CreateUserAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(IdentityUser user, string roleName)
        {
            var result = await userManager.AddToRoleAsync(user, roleName);
            return result;
        }

        public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
        {
            var result = await userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            return result;
        }

        public async Task DeleteAsync(IdentityUser user)
        {
            await userManager.DeleteAsync(user);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            await userManager.DeleteAsync(user);
        }

        public IEnumerable<IdentityUser> Find(Expression<Func<IdentityUser, bool>> predicate)
        {
            var users = userManager.Users.Where(predicate).AsEnumerable();
            return users;
        }

        public async Task<IdentityUser> FindByNameAsync(string name)
        {
            var user = await userManager.FindByNameAsync(name);
            return user;
        }

        public async Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            var users = userManager.Users.AsEnumerable();
            return users;
        }

        public async Task<IdentityUser> GetByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task UpdateAsync(IdentityUser user)
        {
            await userManager.UpdateAsync(user);
        }
    }
}