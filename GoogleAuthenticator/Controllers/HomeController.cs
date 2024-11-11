using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoogleAuthenticator.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() => View();

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Private()
    {
        var context = HttpContext;
        return View();
    }
}