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

    // ==== ADD SESSION ====

    [HttpGet]
    public IActionResult Add_session(int id)
    {
        var selected_course = _context.Courses
        .Include(c => c.Teacher)
        .FirstOrDefault(course => course.Id == id);

        if (selected_course == null)
        {
            return NotFound();
        }
        var session = new CourseSession
        {
            CourseId = selected_course.Id
        };

        ViewBag.CourseName = selected_course.Name;
        ViewBag.TeacherName = selected_course.Teacher?.FullName ?? "Ei opettajaa";

        return View(session);
    }
    [HttpPost]
    public IActionResult Add_session(CourseSession coursesession)
    {
        coursesession.Id = 0;
        
        Console.WriteLine($"Id: {coursesession.Id}");
        Console.WriteLine($"CourseId: {coursesession.CourseId}");
        Console.WriteLine($"WeekDay: {coursesession.WeekDay}");
        Console.WriteLine($"Time_of_start: {coursesession.Time_of_start}");
        Console.WriteLine($"Time_of_end: {coursesession.Time_of_end}");
        Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    if (error.Exception != null)
                        Console.WriteLine($"Exception: {error.Exception.Message}");
                }
            }
        }

        if (ModelState.IsValid)
        {
            _context.CourseSessions.Add(coursesession);
            _context.SaveChanges();
            Console.WriteLine("Session saved successfully!");
            return RedirectToAction("List_courses");
        }

        Console.WriteLine("ModelState invalid, returning view");
        return View(coursesession);
    }

    // ==== DELETE ==== 
    
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
