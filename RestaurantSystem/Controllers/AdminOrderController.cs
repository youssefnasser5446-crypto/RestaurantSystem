using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Models;

public class AdminOrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminOrderController(ApplicationDbContext context)
    {
        _context = context;
    }

     
    public async Task<IActionResult> Index()
    {
        // سحب الطلبات مع تفاصيلها
        var orders = await _context.Orders.Include(o => o.OrderDetails).OrderByDescending(o => o.Id).ToListAsync();
        return View(orders);
    }



    public IActionResult MonthlySales(int? year)
    {
        int selectedYear = year ?? DateTime.Now.Year;
        ViewBag.SelectedYear = selectedYear;

        var salesReport = _context.OrderDetails
            .Include(d => d.Order)
            .Where(d => d.Order.OrderDate.Year == selectedYear && d.Order.Status == "تم التوصيل")
            .GroupBy(d => new { d.Order.OrderDate.Month, d.ItemName })
            .Select(g => new MonthlySalesViewModel
            {
                Month = g.Key.Month,
                ItemName = g.Key.ItemName,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = (double)g.Sum(x => x.Quantity * x.Price)
            })
            .ToList();

        return View(salesReport);
    }
 


    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = newStatus;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}