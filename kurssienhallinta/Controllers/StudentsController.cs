using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class StudentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(AppDbContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }


    [HttpGet]
    public IActionResult List()
    {
        var students = _context.Students.ToList();
        return View(students);
    }


    [HttpGet]
    public IActionResult Add_student()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Add_student(Student student)
    {
        if (ModelState.IsValid)
        {

            student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        return View(student);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
