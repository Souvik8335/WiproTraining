using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoConnect.Services;
using Model;
using BCrypt.Net;
using System.Threading.Tasks;

namespace DoConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly DoContext _db;
        private readonly Service _jwt;

        public AccountController(DoContext db, Service jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Find user by username
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(dto);
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(dto);
            }

            // Generate JWT token (or handle session)
            var token = _jwt.CreateToken(user);

            // For MVC, you might want to store the token in session or cookie
            // For simplicity, redirect to home or admin based on role
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin/Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
