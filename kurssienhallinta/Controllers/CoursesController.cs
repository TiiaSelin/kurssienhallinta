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
        ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "FullName");
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

        ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "FullName");
        ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");

        return View(course);
    }
    
     [HttpGet]
    public IActionResult Edit_course(int id)
    {
        var selected_course = _context.Courses.FirstOrDefault(course => course.Id == id);

        if (selected_course == null)
        {
            return NotFound();
        }

        ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "FullName");
        ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");

        return View(selected_course);
    }
    [HttpPost]
    public IActionResult Edit_course(Course course)
    {

        course.Day_of_start = DateTime.SpecifyKind(
            course.Day_of_start,
            DateTimeKind.Utc
            );
        course.Day_of_end = DateTime.SpecifyKind(
            course.Day_of_end,
            DateTimeKind.Utc
            );

        if (!ModelState.IsValid)
        {
            ViewBag.Teachers = new SelectList(_context.Teachers, "Id", "FullName");
            ViewBag.Rooms = new SelectList(_context.Rooms, "Id", "Name");
            return View(course);
        }

        _context.Courses.Update(course);
        _context.SaveChanges();
        return RedirectToAction("List_courses");
    }

    [HttpGet]
    public IActionResult Details_course(int id)
    {
        var course = _context.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Room)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefault(c => c.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpPost]
    public IActionResult Delete_course(int id, bool confirm = false)
    {

        var course = _context.Courses.FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            return NotFound();
        }
        _context.Courses.Remove(course);
        _context.SaveChanges();
        return RedirectToAction("List_courses");
    }

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
