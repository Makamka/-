using Microsoft.AspNetCore.Mvc;
using ECommerceSolution.Data;
using System.Linq;

namespace ECommerceSolution.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductsApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _db.Products.ToList();
            return Ok(products);
        }
    }
}
