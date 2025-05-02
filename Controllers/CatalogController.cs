using System.Linq;
using System.Threading.Tasks;
using ECommerceSolution.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSolution.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Catalog
        // опціонально приймаємо масив categoryIds для фільтрації
        public async Task<IActionResult> Index(int[] categoryIds, string sort)
        {
            // підвантажуємо товари з категоріями
            var products = _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            // фільтрація за вибраними категоріями
            if (categoryIds != null && categoryIds.Any())
            {
                products = products.Where(p =>
                    p.ProductCategories.Any(pc => categoryIds.Contains(pc.CategoryId)));
            }

            // сортування
            products = sort switch
            {
                "price" => products.OrderBy(p => p.Price),
                "rating" => products.OrderByDescending(p => p.Reviews.Average(r => r.Rating)),
                _ => products
            };

            // передаємо список доступних категорій у ViewBag
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategoryIds = categoryIds;

            return View(await products.ToListAsync());
        }
    }
}
