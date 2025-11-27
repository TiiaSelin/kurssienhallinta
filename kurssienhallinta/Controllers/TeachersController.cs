using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models.ViewModels;

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

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var teacher = _context.Teachers.Find(id);

        if (teacher == null)
        {
            return NotFound();
        }

        return View(teacher);
    }


    [HttpPost]
    public IActionResult Edit(Teacher teacher)
    {
        if (ModelState.IsValid)
        {
            _context.Teachers.Update(teacher);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        return View(teacher);
    }

    // ==== DETAILS ====
    [HttpGet]
    public IActionResult Details(int id)
    {
        var teacher = _context.Teachers
            .Include(t => t.Courses)
                .ThenInclude(c => c.Sessions)
            .Include(t => t.Courses)
                .ThenInclude(c => c.Room)
            .FirstOrDefault(t => t.Id == id);

        if (teacher == null)
            return NotFound();

        // Get sessions directly from loaded courses (no DB query needed)
        var sessions = teacher.Courses
            .SelectMany(c => c.Sessions)
            .ToList();

        // Convert to ScheduleItem list
        var scheduleItems = sessions.Select(coursesession => new ScheduleItem
        {
            Id = coursesession.CourseId,
            Name = coursesession.Course.Name,
            Description = coursesession.Course.Description,
            Day_of_start = coursesession.Course.Day_of_start,
            Day_of_end = coursesession.Course.Day_of_end,
            TeacherId = coursesession.Course.TeacherId,
            RoomId = coursesession.Course.RoomId,
            Start_time = coursesession.Time_of_start,
            End_time = coursesession.Time_of_end,
            Weekday = coursesession.WeekDay.ToString()
        }).ToList();

        // Build weekly schedule dictionary grouped by weekday
        var weeklySchedule = scheduleItems
            .GroupBy(si => si.Weekday)
            .ToDictionary(g => g.Key, g => g.OrderBy(si => si.Start_time).ToList());

        var viewModel = new TeacherScheduleViewModel
        {
            Teacher = teacher,
            WeeklySchedule = weeklySchedule
        };

        return View(viewModel);
    }
    // ==== DELETE ====

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var teacher = _context.Teachers.Find(id);
        if (teacher == null)
            return NotFound();

        _context.Teachers.Remove(teacher);
        _context.SaveChanges();

        return RedirectToAction("List");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }



}
