using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogContext _context;

        public AccountController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user != null && VerifyPassword(model.Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Login),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };
                    foreach (var userRole in user.UserRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    }
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Неверный логин или пароль.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Login == model.Login))
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
                    return View(model);
                }
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Пользователь с таким email уже существует.");
                    return View(model);
                }

                var user = new User
                {
                    Login = model.Login,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Автоматически назначаем роль "Пользователь"
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
                if (userRole == null)
                {
                    userRole = new Role { Name = "Пользователь" };
                    _context.Roles.Add(userRole);
                    await _context.SaveChangesAsync();
                }
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id });
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public IActionResult AdminPanel()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ModeratorOrAdmin")]
        public IActionResult ModeratorOrAdmin()
        {
            return View();
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Здесь должна быть логика проверки хеша пароля
            // Для примера — заглушка
            return password == passwordHash;
        }

        private string HashPassword(string password)
        {
            // Здесь должна быть логика хеширования пароля
            // Для примера — заглушка
            return password;
        }
    }

    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}