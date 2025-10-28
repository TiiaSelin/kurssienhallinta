using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class EnrollmentsController : Controller
{
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(ILogger<EnrollmentsController> logger)
    {
        _logger = logger;
    }

    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_enrollment()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
