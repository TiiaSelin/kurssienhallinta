using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class TeachersController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<TeachersController> _logger;

    public TeachersController(AppDbContext context, ILogger<TeachersController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpGet]
    public IActionResult List()
    {
        var teachers = _context.Teachers.ToList();
        return View(teachers);
    }


    [HttpGet]
    public IActionResult Add_teacher()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Add_teacher(Teacher teacher)
    {
        if (ModelState.IsValid)
        {
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        return View(teacher);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
