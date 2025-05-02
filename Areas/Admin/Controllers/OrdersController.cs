using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceSolution.Data;
using ECommerceSolution.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSolution.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrdersController(ApplicationDbContext db) => _db = db;

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                                  .OrderByDescending(o => o.CreatedAt)
                                  .ToListAsync();
            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders
                                 .Include(o => o.OrderItems)
                                   .ThenInclude(oi => oi.Product)
                                 .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Admin/Orders/ChangeStatus/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null && Order.AllStatuses.Contains(newStatus))
            {
                order.Status = newStatus;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
