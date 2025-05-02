using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ECommerceSolution.Models;
using ECommerceSolution.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ECommerceSolution.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext db,               // ← DI контексту
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            // Завантажуємо ролі для кожного користувача
            var userRolesDict = new Dictionary<string, IList<string>>();
            foreach (var u in users)
                userRolesDict[u.Id] = await _userManager.GetRolesAsync(u);

            ViewBag.UserRoles = userRolesDict;
            return View(users);
        }

        // POST: Admin/Users/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // 1) Видаляємо спочатку всі замовлення і їх елементи, що належать цьому користувачу
                var orders = await _db.Orders
                                      .Include(o => o.OrderItems)
                                      .Where(o => o.UserId == id)
                                      .ToListAsync();
                foreach (var o in orders)
                {
                    _db.OrderItems.RemoveRange(o.OrderItems);
                    _db.Orders.Remove(o);
                }
                await _db.SaveChangesAsync();

                // 2) Тепер видаляємо користувача
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/MakeAdmin
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                    await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/RevokeAdmin
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
