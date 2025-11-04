using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class CoursesController : Controller
{

    public IActionResult List()
    {
        return View();
    }

    public IActionResult Add_course()
    {
        return View();
    }

    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ILogger<CoursesController> logger)
    {
        _logger = logger;
    }

}
