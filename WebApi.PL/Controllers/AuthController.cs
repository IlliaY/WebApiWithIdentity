using Microsoft.AspNetCore.Mvc;

namespace WebApi.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public int MyProperty { get; set; }

    }
}