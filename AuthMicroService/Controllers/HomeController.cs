using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        [Authorize]
        [HttpGet("protect")]
        public async Task<IActionResult> Protect()
        {
            return Ok("Protected Controller Test");
        }
    }
}
