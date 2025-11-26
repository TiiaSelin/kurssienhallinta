using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kurssienhallinta.Controllers;

public class CoursesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(AppDbContext context, ILogger<CoursesController> logger)
    {
        _context = context;
        _logger = logger;
    }

[HttpGet]
public IActionResult List_courses()
    {
        var courses = _context.Courses
        .Include(c => c.Teacher)
        .Include(c => c.Room)
        .ToList();
        return View(courses);
    }

    [HttpGet]
    public IActionResult Add_course()
    {
        ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "First_name");
        ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");

        return View();
    }
    
[HttpPost]
public IActionResult Add_course(Course course)
    {
        if (ModelState.IsValid)
        {
            course.Day_of_start = DateTime.SpecifyKind(
                course.Day_of_start,
                DateTimeKind.Utc
            );
            course.Day_of_end = DateTime.SpecifyKind(
                course.Day_of_end,
                DateTimeKind.Utc
            );

            _context.Courses.Add(course);
            _context.SaveChanges();
            return RedirectToAction("List_courses");
        }

        ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "First_name");
        ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");

        return View(course);
    }

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
