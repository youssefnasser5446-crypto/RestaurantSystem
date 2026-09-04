using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantSystem.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        // ربط بالجدول الرئيسي
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public string ItemName { get; set; } // اسم الأكلة
        public int Quantity { get; set; }    // الكمية
        public int Price { get; set; }       // سعر القطعة وقت البيع
    }
}