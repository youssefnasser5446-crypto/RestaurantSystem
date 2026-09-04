using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models; // تأكد من وجود هذا السطر للوصول لموديل User

namespace RestaurantSystem.Models // أو RestaurantSystem.Data حسب تقسيم المجلدات عندك
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // أضف هذا السطر لتعريف جدول المستخدمين
        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}