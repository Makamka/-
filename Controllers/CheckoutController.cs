// Controllers/CheckoutController.cs
using Microsoft.AspNetCore.Mvc;
using ECommerceSolution.Data;
using ECommerceSolution.Models;
using System.Linq;
using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerceSolution.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Checkout
        public IActionResult Index()
        {
            // Повертаємо порожню модель для форми
            return View(new CheckoutViewModel());
        }

        // POST: /Checkout/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Якщо не пройшла валідація — повернути ту саму форму
                return View("Index", vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Створюємо замовлення
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                FullName = vm.FullName,
                Email = vm.Email,
                Phone = vm.Phone,
                Address = vm.Address
            };
            _db.Orders.Add(order);

            // Копіюємо товари з кошика
            var cartItems = _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            foreach (var c in cartItems)
            {
                _db.OrderItems.Add(new OrderItem
                {
                    Order = order,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                });
            }

            // Очищаємо кошик
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return RedirectToAction("Confirmation");
        }

        // GET: /Checkout/Confirmation
        public IActionResult Confirmation()
        {
            ViewBag.Title = "Підтвердження замовлення";
            return View();
        }
    }
}
