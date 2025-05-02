using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ECommerceSolution.Models;
using System.Linq;

[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[Route("api/v1/[controller]")]
public class UsersInMemoryController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersInMemoryController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new {
            u.Id,
            u.UserName,
            u.Email
        }).ToList();

        return Ok(users);
    }
}
