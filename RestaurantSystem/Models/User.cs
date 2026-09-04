using System.ComponentModel.DataAnnotations;

namespace RestaurantSystem.Models
{
    public class User
    {
        // 1. إضافة الـ Id كمفتاح أساسي (ضروري للداتابيز)
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الإسم مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "الإيميل مطلوب")]
        // حافظنا على الـ RegularExpression بتاعك عشان تضمن الدومين صح
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@restaurant\.com$", ErrorMessage = "لازم الإيميل ينتهي بـ @restaurant.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}