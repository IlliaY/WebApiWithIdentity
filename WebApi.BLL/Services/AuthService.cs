using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IValidator<UserLoginModel> validatorLogin;
        private readonly IValidator<UserRegisterModel> validatorRegister;

        public AuthService
        (
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            IJwtCreationService jwtCreationService,
            IValidator<UserLoginModel> validatorLogin,
            IValidator<UserRegisterModel> validatorRegister
        )
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.jwtCreationService = jwtCreationService;
            this.validatorLogin = validatorLogin;
            this.validatorRegister = validatorRegister;
        }

        /// <summary>
        /// Method takes a username and password, checks if the user exists and if the password is correct, then
        /// creates a JWT token with the user's roles and returns it
        /// </summary>
        /// <param name="UserLoginModel">This is a model class that contains the username and password.</param>
        /// <returns>
        /// Response object with status and message with token
        /// </returns>
        public async Task<TokenDTO> LoginAsync(UserLoginModel userLogin)
        {
            await validatorLogin.ValidateAndThrowAsync(userLogin);
            var user = await userManager.FindByNameAsync(userLogin.UserName);
            if (user == null || !await userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                throw new Exception("Wrong username or password");
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

            return new TokenDTO
            {
                Token = handler.WriteToken(token)
            };
        }


        /// <summary>
        /// Method creates a new user, adds the user to the "User" role, and returns a response object
        /// </summary>
        /// <param name="UserRegisterModel"></param>
        /// <returns>
        /// A response object with a status and message.
        /// </returns>
        public async Task<MessageDTO> RegisterAsync(UserRegisterModel userRegister)
        {
            await validatorRegister.ValidateAndThrowAsync(userRegister);

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

            return new MessageDTO() { Message = "User created successfully!" };
        }

        /// <summary>
        /// Method creates a new user with the role of Admin
        /// </summary>
        /// <param name="UserRegisterModel">This is a model class that contains the properties of the
        /// user that we want to register.</param>
        /// <returns>
        /// A response object with a status and message.
        /// </returns>
        public async Task<MessageDTO> RegisterAdminAsync(UserRegisterModel userRegister)
        {
            await validatorRegister.ValidateAndThrowAsync(userRegister);

            var adminExists = await userManager.FindByNameAsync(userRegister.UserName);

            if (adminExists != null)
            {
                throw new Exception("Admin already exists");
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

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            await userManager.AddToRoleAsync(user, "Admin");

            return new MessageDTO() { Message = "Admin created successfully!" };
        }

    }
}