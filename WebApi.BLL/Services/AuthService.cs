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
using WebApi.BLL.Exceptions;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Models;
using WebApi.DAL.Interfaces;

namespace WebApi.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly IJwtCreationService jwtCreationService;
        private readonly IValidator<UserLoginModel> validatorLogin;
        private readonly IValidator<UserRegisterModel> validatorRegister;

        public AuthService
        (
            IConfiguration configuration,
            IJwtCreationService jwtCreationService,
            IValidator<UserLoginModel> validatorLogin,
            IValidator<UserRegisterModel> validatorRegister,
            IUnitOfWork unitOfWork)
        {
            this.configuration = configuration;
            this.jwtCreationService = jwtCreationService;
            this.validatorLogin = validatorLogin;
            this.validatorRegister = validatorRegister;
            this.unitOfWork = unitOfWork;
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

            var user = await unitOfWork.UserRepository.FindByNameAsync(userLogin.UserName);

            if (user == null || !await unitOfWork.UserRepository.CheckPasswordAsync(user, userLogin.Password))
            {
                throw new AuthentificationException("Wrong username or password");
            }

            var userRoles = await unitOfWork.UserRepository.GetRolesAsync(user);

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

            var userExists = await unitOfWork.UserRepository.FindByNameAsync(userRegister.UserName);

            if (userExists != null)
            {
                throw new AuthentificationException("User already exists");
            }

            IdentityUser user = new IdentityUser
            {
                UserName = userRegister.UserName,
                Email = userRegister.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await unitOfWork.UserRepository.CreateUserAsync(user, userRegister.Password);

            if (!result.Succeeded)
            {
                throw new AuthentificationException("Error creating user");
            }

            if (!await unitOfWork.RoleRepository.RoleExistsAsync("User"))
            {
                await unitOfWork.RoleRepository.CreateRoleAsync(new IdentityRole("User"));
            }

            await unitOfWork.UserRepository.AddToRoleAsync(user, "User");

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

            var adminExists = await unitOfWork.UserRepository.FindByNameAsync(userRegister.UserName);

            if (adminExists != null)
            {
                throw new AuthentificationException("Admin already exists");
            }

            IdentityUser user = new IdentityUser
            {
                UserName = userRegister.UserName,
                Email = userRegister.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await unitOfWork.UserRepository.CreateUserAsync(user, userRegister.Password);

            if (!result.Succeeded)
            {
                throw new AuthentificationException("Error creating user");
            }

            if (!await unitOfWork.RoleRepository.RoleExistsAsync("Admin"))
            {
                await unitOfWork.RoleRepository.CreateRoleAsync(new IdentityRole("Admin"));
            }

            await unitOfWork.UserRepository.AddToRoleAsync(user, "Admin");

            return new MessageDTO() { Message = "Admin created successfully!" };
        }

    }
}