using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public int TotalPrice { get; set; }

        public string Status { get; set; } = "جاري التجهيز";

        public string? UserEmail { get; set; }

        // أضف ده عشان تعرف الأوردر اتعمل إمتى
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? UserId { get; set; }

        // الربط بالجدول التاني (علاقة 1 إلى متعدد)
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}