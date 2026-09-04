using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantSystem.Controllers
{
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FoodController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Drinks() => View();
        public IActionResult Sweets() => View();
        public IActionResult Grill() => View();

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

       

        // 1. تعديل إضافة الصنف للسلة (حفظ السعر في السشن)
        [HttpPost]
        public IActionResult AddToCart(string itemName, int price, int quantity)
        {
            // حفظ الكمية
            HttpContext.Session.SetInt32(itemName, quantity);

            // حفظ سعر القطعة الواحدة عشان نستخدمه في الـ Checkout
            HttpContext.Session.SetInt32(itemName + "_Price", price);

            int total = HttpContext.Session.GetInt32("TotalPrice") ?? 0;
            total += (price * quantity);
            HttpContext.Session.SetInt32("TotalPrice", total);

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var total = HttpContext.Session.GetInt32("TotalPrice") ?? 0;
            if (total == 0) return RedirectToAction("Grill");
            ViewBag.TotalPrice = total;
            return View();
        }

        // 2. تعديل إتمام الطلب (سحب السعر من السشن وتخزينه في الداتابيز)
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    userEmail = "Guest";
                }

                order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Guest";
                order.UserEmail = userEmail;
                order.OrderDate = DateTime.Now;
                order.Status = "قيد الانتظار";

                if (order.TotalPrice == 0)
                {
                    order.TotalPrice = HttpContext.Session.GetInt32("TotalPrice") ?? 0;
                }

                order.OrderDetails = new List<OrderDetail>();

                foreach (var key in HttpContext.Session.Keys)
                {
                    // نتجاهل مفاتيح السشن اللي مش أسماء أصناف
                    if (key == "TotalPrice" || key == "UserEmail" || key == "CustomerName" || key.EndsWith("_Price")) continue;

                    var qty = HttpContext.Session.GetInt32(key);

                    // سحب السعر المخزن لهذا الصنف تحديداً
                    var itemPrice = HttpContext.Session.GetInt32(key + "_Price") ?? 0;

                    if (qty.HasValue && qty > 0)
                    {
                        order.OrderDetails.Add(new OrderDetail
                        {
                            ItemName = key,
                            Quantity = qty.Value,
                            Price = itemPrice // هنا السعر هيتسجل صح في قاعدة البيانات
                        });
                    }
                }

                _db.Orders.Add(order);
                _db.SaveChanges();

                HttpContext.Session.Clear();

                return RedirectToAction("Success", new { id = order.Id });
            }
            catch (Exception ex)
            {
                return Content($"حصلت مشكلة أثناء إتمام الطلب: {ex.Message}");
            }
        }

        public IActionResult Success(int id)
        {
            var order = _db.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.Id == id);
            if (order == null) return Content("الطلب غير موجود");
            return View(order);
        }

        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null && order.Status != "جاري التجهيز" && order.Status != "تم التوصيل")
            {
                _db.Orders.Remove(order);
                _db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Success", new { id = orderId });
        }

        public IActionResult AdminOrders()
        {
            var orders = _db.Orders.Include(o => o.OrderDetails).OrderByDescending(o => o.Id).ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string newStatus)
        {
            var order = _db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _db.SaveChanges();
                return RedirectToAction("AdminOrders");
            }
            return Content("الطلب غير موجود");
        }
    }
}