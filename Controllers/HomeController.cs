using Microsoft.AspNetCore.Mvc;
using ECommerceSolution.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSolution.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Завантажуємо перші 5 продуктів для слайдера
            ViewBag.Slider = await _db.Products.Take(5).ToListAsync();

            // Завантажуємо всі категорії
            ViewBag.Categories = await _db.Categories.ToListAsync();

            // Завантажуємо продукти, відсортовані за кількістю відгуків
            ViewBag.Trending = await _db.Products
                                        .OrderByDescending(p => p.Reviews.Count)
                                        .Take(8)
                                        .ToListAsync();

            // Завантажуємо продукти, відсортовані за ціною, з перетворенням на double
            ViewBag.Popular = await _db.Products
                                        .OrderByDescending(p => (double)p.Price) // Перетворення на double для сортування
                                        .Take(8)
                                        .ToListAsync();

            return View();
        }
    }
}
