using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {
        [HttpGet("TestToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult TestToken()
        {
            return Ok("Bienvenue!");
        }

        [HttpGet("Anonymous")]
        public IActionResult TestAnonymous()
        {
            return Ok("Vous êtes anonyme!");
        }
    }
}
