using BLL_DBSlide.Entities;
using BLL_DBSlide.Entities.Enums;
using Common_DBSlide.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository<User, Role> _userRepository;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository<User, Role> userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id) {
            return Ok(_userRepository.Get(id));
        }

        [HttpPost]
        public IActionResult Login([FromForm] string email, [FromForm] string password)
        {
            Guid? id = _userRepository.CheckPassword(email, password);
            if(id is null) return Unauthorized();
            return Ok(new { id, token = CreateToken((Guid)id) });
        }

        private string CreateToken(Guid user_id)
        {
            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));
            SigningCredentials credentials = new SigningCredentials(symmetricKey,SecurityAlgorithms.HmacSha256);
            User user = _userRepository.Get(user_id);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.PrimarySid, user.User_Id.ToString()));
            JwtSecurityToken token = new JwtSecurityToken(
                _config["jwt:issuer"],
                _config["jwt:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials : credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
