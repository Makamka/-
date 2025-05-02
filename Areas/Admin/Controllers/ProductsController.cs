using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ECommerceSolution.Data;
using ECommerceSolution.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSolution.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            var items = await _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .ToListAsync();
            return View(items);
        }

        // GET: /Admin/Products/Create
        public IActionResult Create()
        {
            ViewBag.AllCategories = _context.Categories
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
            return View();
        }

        // POST: /Admin/Products/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel vm, int[] selectedCategoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllCategories = _context.Categories
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                    .ToList();
                return View(vm);
            }

            // обчислити новий унікальний Id
            var existingIds = _context.Products.Select(p => p.Id).ToHashSet();
            int newId = Enumerable.Range(1, existingIds.Count + 1)
                                  .First(i => !existingIds.Contains(i));

            // зберегти зображення
            string fileName = null;
            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(uploads);
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.ImageFile.FileName)}";
                await using var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create);
                await vm.ImageFile.CopyToAsync(stream);
            }

            // створити і зберегти продукт
            var product = new Product
            {
                Id = newId,
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                ImageUrl = fileName != null ? $"/images/{fileName}" : null,
            };
            // зв’язки many-to-many
            foreach (var catId in selectedCategoryIds)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    CategoryId = catId,
                    ProductId = newId
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Products/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                // видалити файл із диска
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imgPath = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }
                // видалити зв’язки
                _context.ProductCategories.RemoveRange(product.ProductCategories);
                // видалити сам продукт
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
