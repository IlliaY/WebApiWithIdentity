using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Models;
using WebApi.PL.Filters;

namespace WebApi.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// The Register method is a POST method that takes in a UserRegisterModel object, validates
        /// it, and then calls the RegisterAsync function in the AuthService
        /// </summary>
        /// <param name="UserRegisterModel">This is the model that will be used to register a
        /// user.</param>
        /// <returns>
        /// The response is being returned.
        /// </returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserRegisterModel userRegister)
        {
            var response = await authService.RegisterAsync(userRegister);
            return Ok(response);
        }

        /// <summary>
        /// Method takes a userLogin object, validates it, and then calls the LoginAsync function in the
        /// authService
        /// </summary>
        /// <param name="UserLoginModel">This is the model that will be used to validate the user's
        /// login credentials.</param>
        /// <returns>
        /// The response is being returned.
        /// </returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginModel userLogin)
        {
            var response = await authService.LoginAsync(userLogin);
            return Ok(response);
        }

        /// <summary>
        /// This method is used to register a new admin user
        /// </summary>
        /// <param name="UserRegisterModel">This is the model that will be used to register a
        /// user.</param>
        /// <returns>
        /// The response is being returned.
        /// </returns>
        [HttpPost]
        [Route("RegisterAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin(UserRegisterModel userRegister)
        {
            var response = await authService.RegisterAdminAsync(userRegister);
            return Ok(response);
        }
    }
}