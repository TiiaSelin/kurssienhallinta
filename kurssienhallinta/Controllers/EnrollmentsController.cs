using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class EnrollmentsController : Controller
{

    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_enrollment()
    {
        return View();
    }
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(ILogger<EnrollmentsController> logger)
    {
        _logger = logger;
    }


}
