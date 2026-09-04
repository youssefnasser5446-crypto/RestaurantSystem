using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using RestaurantSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestaurantSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                var isEmailExist = _context.Users.Any(u => u.Email == model.Email);
                if (isEmailExist)
                {
                    ModelState.AddModelError("Email", "هذا الإيميل مسجل بالفعل!");
                    return View(model);
                }

                _context.Users.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            // 1. التحقق من "الأدمن" الثابت
            if (Email == "admin@restaurant.com" && Password == "admin123")
            {
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "مدير المطعم"),
                    new Claim(ClaimTypes.Email, Email),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("UserType", "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                // --- التعديل الجوهري هنا ---
                // تم التغيير من "Admin" إلى "AdminOrder" ليتوافق مع اسم الكنترولر والمجلد عندك
                return RedirectToAction("Index", "AdminOrder");
            }

            // 2. التحقق من الزبائن في قاعدة البيانات
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim("UserType", "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "الإيميل أو كلمة المرور غير صحيحة، أو الحساب غير موجود.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}