using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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

        [HttpPost]
        [Route("Register")]
        [ValidationFilter]
        public async Task<IActionResult> Register(UserRegisterModel userRegister)
        {
            try
            {
                var response = await authService.RegisterAsync(userRegister);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginModel userLogin)
        {
            try
            {
                var response = await authService.LoginAsync(userLogin);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}