using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoConnect.Services;
using Model;
using BCrypt.Net; // BCrypt.Net-Next
using System.Threading.Tasks;
using DoConnect;

namespace DoConnect.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DoContext _db;
        private readonly Service _jwt;

        public AuthController(DoContext db, Service jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        //================  In services auth.cs is were generated the jwt token ========
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Check if username already exists
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return Conflict("User already exists");

            // Map DTO to User entity
            var user = new User
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role ?? "User" // default role jodi user role choose na kore ota auto user hobe
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { user.Username, user.Role });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Find user by username
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return Unauthorized("Invalid username or password");

            // =========  Verify password ==============
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid username or password");

            // Generate JWT token
            var token = _jwt.CreateToken(new User
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            });

            return Ok(new
            {
                token,
                user = new { user.UserId, user.Username, user.Role }
            });
        }
    }
}
