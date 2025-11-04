using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class TeachersController : Controller
{
    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_teacher()
    {
        return View();
    }
    private readonly ILogger<TeachersController> _logger;

    public TeachersController(ILogger<TeachersController> logger)
    {
        _logger = logger;
    }


}
