using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Repositories
{
    class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task AddAsync(IdentityRole role)
        {
            await CreateRoleAsync(role);
        }

        public async Task<IdentityResult> CreateRoleAsync(IdentityRole role)
        {
            var result = await roleManager.CreateAsync(role);
            return result;
        }

        public async Task DeleteAsync(IdentityRole role)
        {
            await roleManager.DeleteAsync(role);
        }

        public async Task DeleteByIdAsync(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
        }

        public IEnumerable<IdentityRole> Find(Expression<Func<IdentityRole, bool>> predicate)
        {
            var roles = roleManager.Roles.Where(predicate).AsEnumerable();
            return roles;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllAsync()
        {
            var roles = roleManager.Roles.AsEnumerable();
            return roles;
        }

        public Task<IdentityRole> GetByIdAsync(string id)
        {
            var role = roleManager.FindByIdAsync(id);
            return role;
        }

        public Task<bool> RoleExistsAsync(string roleName)
        {
            var result = roleManager.RoleExistsAsync(roleName);
            return result;
        }

        public async Task UpdateAsync(IdentityRole role)
        {
            await roleManager.UpdateAsync(role);
        }
    }
}