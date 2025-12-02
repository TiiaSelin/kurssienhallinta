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
    public IActionResult Details(int id, int weekOffset = 0)
    {
        var teacher = _context.Teachers // Vaihtuu
            .Include(teacher => teacher.Courses)
                .ThenInclude(course => course.Sessions)
            .Include(teacher => teacher.Courses)
                .ThenInclude(course => course.Room)
            .FirstOrDefault(teacher => teacher.Id == id);

        if (teacher == null) // Vaihtuu
            return NotFound();

        var today = DateTime.Now;
        var daysOfWeek = (int)today.DayOfWeek;
        var mondayThisWeek = today.AddDays(-(daysOfWeek == 0 ? 6 : daysOfWeek - 1)).Date;
        var weekStart = mondayThisWeek.AddDays(weekOffset * 7);
        var weekEnd = weekStart.AddDays(6);

        var sessions = teacher.Courses // Vaihtuu
            .SelectMany(course => course.Sessions)
            .ToList();

        var scheduleItems = sessions.Select(cs => new ScheduleItem // Ei vaihdu
        {
            Id = cs.CourseId,
            Name = cs.Course.Name,
            Description = cs.Course.Description,
            Day_of_start = cs.Course.Day_of_start,
            Day_of_end = cs.Course.Day_of_end,
            TeacherId = cs.Course.TeacherId,
            RoomId = cs.Course.RoomId,
            Start_time = cs.Time_of_start,
            End_time = cs.Time_of_end,
            Weekday = cs.WeekDay.ToString(),
            Room = cs.Course.Room
        }).ToList();

        var weeklySchedule = scheduleItems
            .GroupBy(si => si.Weekday)
            .ToDictionary(g => g.Key, g => g.OrderBy(si => si.Start_time).ToList());

        var allDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        foreach (var day in allDays)
        {
            if (!weeklySchedule.ContainsKey(day))
                weeklySchedule[day] = new List<ScheduleItem>();
        }

        var viewModel = new TeacherScheduleViewModel // Vaihtuu
        {
            Teacher = teacher, // Vaihtuu
            WeeklySchedule = weeklySchedule,
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            WeekOffset = weekOffset
        };

        return View(viewModel);
    }
        /*
        Palautetaan ViewModel, joka sisältää Dictionary<string, List<ScheduleItem>> tähän tyyliin:

        WeeklySchedule = {
            "Monday": [
                ScheduleItem { Name="Math", Start_time=09:00, End_time=11:00 },
                ScheduleItem { Name="Physics", Start_time=14:00, End_time=15:00 }
            ],
            "Tuesday": [
                ScheduleItem { Name="English", Start_time=10:00, End_time=12:00 }
            ],   
         */
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
