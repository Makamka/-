using Microsoft.AspNetCore.Mvc;
using ECommerceSolution.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSolution.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db) { _db = db; }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}