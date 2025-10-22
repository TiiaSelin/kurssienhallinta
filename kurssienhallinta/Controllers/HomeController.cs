using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Kurssit()
    {
        return View();
    }

    public IActionResult Tilat()
    {
        return View();
    }
    
    public IActionResult Opiskelijat()
    {
        return View();
    }
    public IActionResult Opettajat()
    {
        return View();
    }

    public IActionResult Kirjautumiset()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
