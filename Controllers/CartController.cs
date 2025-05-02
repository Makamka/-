using ECommerceSolution.Data;
using ECommerceSolution.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly ApplicationDbContext _db;
    public CartController(ApplicationDbContext db) { _db = db; }

    private string CartSession => HttpContext.Session.Id;

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var items = _db.CartItems
                       .Where(c => c.UserId == userId)
                       .Include(c => c.Product)
                       .ToList();
        return View(items);
    }

    [HttpPost]
    public IActionResult Add(int productId, int quantity = 1, string returnUrl = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var item = _db.CartItems
                 .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
        if (item == null)
        {
            _db.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = quantity });
        }
        else
        {
            item.Quantity += quantity;
        }
        _db.SaveChanges();
        TempData["ShowGoToCart"] = "true";
        return Redirect(returnUrl ?? Url.Action("Index"));
    }

    [HttpPost]
    public IActionResult Remove(int id, string returnUrl = null)
    {
        var item = _db.CartItems.Find(id);
        if (item != null) { _db.CartItems.Remove(item); _db.SaveChanges(); }
        return Redirect(returnUrl ?? Url.Action("Index"));
    }
}
