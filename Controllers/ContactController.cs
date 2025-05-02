using Microsoft.AspNetCore.Mvc;
namespace ECommerceSolution.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index() => View();
        [HttpPost]
        public IActionResult Send(string name, string email, string message)
        {
            // send email logic
            return RedirectToAction("Index");
        }
    }
}