using Microsoft.AspNetCore.Mvc;

namespace SaturnService;

public class SportsService : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}