using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class TeachersController : Controller
{
    private readonly ILogger<TeachersController> _logger;

    public TeachersController(ILogger<TeachersController> logger)
    {
        _logger = logger;
    }

    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_teacher()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
