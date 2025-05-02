using Microsoft.AspNetCore.Mvc;
using ECommerceSolution.Data;      // ваш DbContext
using Microsoft.AspNetCore.Authorization;

namespace ECommerceSolution.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        // 1) Інжектимо контекст через конструктор
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        // 2) У методі Index рахуємо
        public IActionResult Index()
        {
            ViewBag.UserCount = _db.Users.Count();
            ViewBag.ProductCount = _db.Products.Count();
            return View();
        }
    }
}

