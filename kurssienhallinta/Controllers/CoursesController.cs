using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class CoursesController : Controller
{
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ILogger<CoursesController> logger)
    {
        _logger = logger;
    }

    public IActionResult List()
    {
        return View();
    }

    public IActionResult Add_course()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
