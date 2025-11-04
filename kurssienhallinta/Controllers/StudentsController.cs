using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class StudentsController : Controller
{

    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_student()
    {
        return View();
    }
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(ILogger<StudentsController> logger)
    {
        _logger = logger;
    }



}
