using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Security.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "User, Admin")] // Admin OU User
        //[Authorize(Roles ="Moderator")] // Moderateur ET (Admin OU User)
        public IActionResult Get()
        {
            return Ok("Authorized");
        }
    }
}
