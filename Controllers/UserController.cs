using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Controllers
{
    public class UserController : Controller
    {
        private readonly BlogContext _context;

        public UserController(BlogContext context)
        {
            _context = context;
        }

        // GET: User
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: User/Details/5
        public IActionResult Details(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create (Регистрация)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                // Проверка уникальности логина и email
                if (_context.Users.Any(u => u.Login == user.Login))
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует.");
                    return View(user);
                }
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже существует.");
                    return View(user);
                }
                // Хеширование пароля
                user.PasswordHash = HashPassword(user.PasswordHash);
                // Автоматически присваиваем базовую роль "Пользователь"
                _context.Users.Add(user);
                _context.SaveChanges();

                // Создаём роль "Пользователь", если её ещё нет
                var userRole = _context.Roles.FirstOrDefault(r => r.Name == "Пользователь");
                if (userRole == null)
                {
                    userRole = new Role { Name = "Пользователь" };
                    _context.Roles.Add(userRole);
                    _context.SaveChanges();
                }
                // Присваиваем роль пользователю
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id });
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var dbUser = _context.Users.FirstOrDefault(u => u.Id == id);
                if (dbUser == null)
                {
                    return NotFound();
                }
                // Проверка уникальности логина и email
                if (_context.Users.Any(u => u.Login == user.Login && u.Id != id))
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует.");
                    return View(user);
                }
                if (_context.Users.Any(u => u.Email == user.Email && u.Id != id))
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже существует.");
                    return View(user);
                }
                dbUser.Login = user.Login;
                dbUser.Email = user.Email;
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    dbUser.PasswordHash = HashPassword(user.PasswordHash);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // Вспомогательный метод для хеширования пароля
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}