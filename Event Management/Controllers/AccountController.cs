using Microsoft.AspNetCore.Mvc;
using Event_Management.Data;
using Event_Management.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Event_Management.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // احفظ بيانات المستخدم في السيشن
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserType", user.UserType ?? "User");

            // إعادة توجيه حسب نوع المستخدم
            if (user.UserType == "Admin")
                return RedirectToAction("Index", "Dashboard"); // لو عندك لوحة تحكم

            return RedirectToAction("Create", "Events"); // للمستخدم العادي
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login");

            return View();
        }



        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View();
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Create", "Events");
        }
    }

}
