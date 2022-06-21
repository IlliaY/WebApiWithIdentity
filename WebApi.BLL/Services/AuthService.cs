using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Models;

namespace WebApi.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IJwtCreationService jwtCreationService;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration configuration, IJwtCreationService jwtCreationService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.jwtCreationService = jwtCreationService;
        }

        public async Task<Response> LoginAsync(UserLoginModel userLogin)
        {
            var user = await userManager.FindByNameAsync(userLogin.UserName);
            if (user == null || !await userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                throw new Exception("Invalid username or password");
            }
            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var handler = new JwtSecurityTokenHandler();

            var token = jwtCreationService.GenerateToken(claims);

            return new Response
            {
                Status = "Success",
                Message = handler.WriteToken(token)
            };
        }

        public async Task<Response> RegisterAsync(UserRegisterModel userRegister)
        {
            var userExists = await userManager.FindByNameAsync(userRegister.UserName);

            if (userExists != null)
            {
                throw new Exception("User already exists");
            }

            IdentityUser user = new IdentityUser
            {
                UserName = userRegister.UserName,
                Email = userRegister.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, userRegister.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Error creating user");
            }

            if (!result.Succeeded)
            {
                var errorList = new List<string>();
                foreach (var error in result.Errors)
                {
                    errorList.Add(error.Description);
                }
                throw new Exception(JsonSerializer.Serialize(errorList));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            await userManager.AddToRoleAsync(user, "User");

            return new Response() { Status = "Success", Message = "User created successfully!" };
        }
    }
}