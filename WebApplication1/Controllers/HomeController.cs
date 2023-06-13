using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    public IActionResult Main()
    {
        return View();
    }
}