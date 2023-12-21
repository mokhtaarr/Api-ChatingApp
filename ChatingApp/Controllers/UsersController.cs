using ChatingApp.Data;
using ChatingApp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            IEnumerable<AppUser> users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return Ok(await _context.Users.FindAsync(id));
        }
    }
}
