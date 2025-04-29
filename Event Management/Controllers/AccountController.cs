using Microsoft.AspNetCore.Mvc;
using Event_Management.Data;
using Event_Management.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Event_Management.Attributes;




namespace Event_Management.Controllers
{
    public class AccountController : Controller

    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }




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

            // ✅ تسجيل بيانات المستخدم في السيشن
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserType", user.UserType ?? "User"); // نحفظ النوع لو موجود، ولو مش موجود نخليه "User" عادي

            // ✅ إعادة توجيه حسب نوع المستخدم
            if (user.UserType == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin"); // لو Admin يروح للوحة التحكم
            }

            return RedirectToAction("Index", "Home"); // لو User عادي يروح للصفحة الرئيسية
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // ✅ تحقق إن الإيميل مش مستخدم من يوزر تاني
            if (_context.Users.Any(u => u.Email == updatedUser.Email && u.UserId != userId))
            {
                ModelState.AddModelError("Email", "This email is already used by another account.");
                return View(updatedUser);
            }

            // ✅ تحقق من قوة الباسورد
            if (string.IsNullOrWhiteSpace(updatedUser.Password) || updatedUser.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
                return View(updatedUser);
            }

            // ✅ التعديل الفعلي
            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            _context.SaveChanges();

            // تحدث السيشن بالاسم الجديد
            HttpContext.Session.SetString("UserName", user.FullName);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
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
            HttpContext.Session.SetString("UserType", user.UserType ?? "User");

            return RedirectToAction("Index", "Home");
        }

    }

}
