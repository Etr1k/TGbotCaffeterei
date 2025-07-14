using CoffeBotAPI.Data;
using CoffeBotAPI.Data.APIdata;
using CoffeBotAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using User = CoffeBotAPI.Model.User;

namespace CoffeeBot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existing != null) return Ok(existing);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}