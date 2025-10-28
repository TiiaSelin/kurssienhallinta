using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class StudentsController : Controller
{
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(ILogger<StudentsController> logger)
    {
        _logger = logger;
    }

    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_student()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
