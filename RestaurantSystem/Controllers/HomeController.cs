using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models; // تأكد من استدعاء المودلز
using System.Linq;

namespace RestaurantSystem.Controllers
{
    public class HomeController : Controller
    {
        // أضف السطرين دول هنا
        private readonly ApplicationDbContext _db;

       

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }



        public IActionResult Index()
        {
            // 1. نجيب إيميل المستخدم من السيشين (عشان نعرف مين اللي فاتح الصفحة)
            string userEmail = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(userEmail))
            {
                // 2. بنروح للداتابيز نجيب أحدث طلب فعلي مسجل لهذا الإيميل
                // هنا الاعتماد كلياً على الجدول اللي في SQL وليس على الـ ViewBag فقط
                var lastOrder = _db.Orders
                    .Where(o => o.UserEmail == userEmail)
                    .OrderByDescending(o => o.Id)
                    .FirstOrDefault();

                // 3. بنبعت الموديل (الطلب نفسه) للـ View عشان يتعرض
                // كدة البيانات جاية من "أصل" الداتابيز
                return View(lastOrder);
            }

            return View();
        }
    }
}