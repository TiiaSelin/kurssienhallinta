using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class EnrollmentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<EnrollmentsController> _logger;

    public EnrollmentsController(AppDbContext context, ILogger<EnrollmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult List()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Add_enrollment()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Add_enrollment(Enrollment enrollment)
    {
        if (ModelState.IsValid)
        {
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        return View(enrollment);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
