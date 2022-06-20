using Microsoft.AspNetCore.Mvc;

namespace WebApi.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World");
        }
    }
}