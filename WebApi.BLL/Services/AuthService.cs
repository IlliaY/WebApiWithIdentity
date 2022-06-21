using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<JwtSecurityToken> LoginAsync(UserLoginModel userLogin)
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

            var token = GetToken(claims);

            return token;
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

            return new Response() { Status = "Succes", Message = result.Errors.ToString() };
        }

        private JwtSecurityToken GetToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}