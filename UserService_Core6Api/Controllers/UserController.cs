using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService_Core6Api.Models;

namespace UserService_Core6Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UsersDbContext _context;
        public UserController(UsersDbContext dbContext) {
            _context = dbContext;
        }


        [Authorize]
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IEnumerable<User>> GetUsers() {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpPost]
        [Route("Profile")]
        public async Task<IEnumerable<User>> Profile(User user=null)
        {
            return await _context.Users.ToListAsync();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<User>> UserRegistration([FromBody]User user)
        {
            var result =  await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new { user = result.Entity} );
        }

        [AllowAnonymous]
        [HttpGet("Test")]
        public async Task<ActionResult<int>> TestService()
        {
            try
            {
                var nou = await _context.Users.CountAsync();
                if (nou > 0)
                {
                    return Ok(new { count = nou });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
            return NotFound();
        }
    }
}
