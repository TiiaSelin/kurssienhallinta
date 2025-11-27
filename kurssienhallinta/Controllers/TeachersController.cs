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
                .ThenInclude(c => c.Room)
            .FirstOrDefault(t => t.Id == id);

        if (teacher == null)
            return NotFound();

        // Get course IDs taught by the teacher
        var courseIds = teacher.Courses.Select(c => c.Id).ToList();

        // Load all CourseSessions for this teacher
        var sessions = _context.CourseSessions
            .Include(cs => cs.Course)
            .Where(cs => courseIds.Contains(cs.CourseId))
            .ToList();

        // Convert to ScheduleItem list
        var scheduleItems = sessions.Select(cs => new ScheduleItem
        {
            Id = cs.CourseId,
            Name = cs.Course.Name,
            Description = cs.Course.Description,
            Day_of_start = cs.Course.Day_of_start,
            Day_of_end = cs.Course.Day_of_end,
            TeacherId = cs.Course.TeacherId,
            RoomId = cs.Course.RoomId,
            Start_time = cs.Time_of_Start,
            End_time = cs.Time_of_End
        }).ToList();

        // Build weekly schedule dictionary grouped by weekday
        var weeklySchedule = scheduleItems
            .GroupBy(si => si.Weekday = sessions
                .First(s => s.CourseId == si.Id && s.Time_of_Start == si.Start_time)
                .Weekday)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(si => si.Start_time).ToList()
            );

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
