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
            .Include(teacher => teacher.Courses)  // Load related courses
                 .ThenInclude(course => course.Room)  // Load rooms through courses
            .FirstOrDefault(teacher => teacher.Id == id);

        if (teacher == null)
            return NotFound();

        var courseItems = teacher.Courses

       .Select(course => new ScheduleItem
       {
           Id = course.Id,
           Name = course.Name,
           Description = course.Description,
           Day_of_start = course.Day_of_start,
           Day_of_end = course.Day_of_end,
           TeacherId = course.TeacherId,
           RoomId = course.RoomId,
           Start_time = course.Start_time,
           End_time = course.End_time
       })
       .ToList();

        var weeklySchedule = courseItems
        .GroupBy(course => course.Day_of_start.DayOfWeek.ToString())
        .ToDictionary(
            group => group.Key,
            group => group.OrderBy(course => course.Start_time).ToList()
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
