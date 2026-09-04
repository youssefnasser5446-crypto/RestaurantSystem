namespace RestaurantSystem.Models
{
    public class MonthlySalesViewModel
    {
        public int Month { get; set; }
        public string ItemName { get; set; }
        public int TotalQuantity { get; set; }
        public double TotalRevenue { get; set; }
    }
}