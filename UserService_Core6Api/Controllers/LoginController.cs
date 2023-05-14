using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService_Core6Api.Models;

namespace UserService_Core6Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        IConfiguration _config;
        UsersDbContext _context;    
        public LoginController(IConfiguration configuration, UsersDbContext context)
        {
            _config = configuration;
            _context = context; 
        }

        [AllowAnonymous]
        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser([FromBody]User user)
        {
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);
            if (userExists !=null)
            {
                return Ok( new { user = userExists });
            }
            return NotFound("User does not exists");
        }
        
        private User AuthenticateUser(User user)
        {
            var userExists = _context.Users.Any(u => u.Email == user.Email && u.Password == user.Password);  
            if (userExists) {
                return user;
            }
            return null;
            
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimsIdentity.DefaultIssuer, user.Email),
                new Claim("owner", "prasad.dwd@gmail.com")
            };
            
            var token = new JwtSecurityToken(
                issuer:_config["Jwt:Issuer"],
                audience:_config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(User user)
        {
            IActionResult response = Unauthorized();
            var _user = AuthenticateUser(user);
            if (_user != null)
            {
                var token = GenerateJwtToken(_user);
                response = Ok(new {token = token});
            }
            return response;
        }       

    }
}
