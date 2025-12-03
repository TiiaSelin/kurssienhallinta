using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

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

    // ==== LIST ====
    [HttpGet]
    public IActionResult List()
    {
        var students = _context.Students.ToList();
        return View(students);
    }

    // ==== ADD ====
    [HttpGet]
    public IActionResult Add_student() => View();

    [HttpPost]
    public IActionResult Add_student(Student student)
    {
        if (!ModelState.IsValid) return View(student);

        student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

        _context.Students.Add(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    // ==== EDIT ====
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return NotFound();
        return View(student);
    }

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (!ModelState.IsValid) return View(student);

        student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

        _context.Students.Update(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    // ==== DELETE ====
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return NotFound();

        _context.Students.Remove(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    // ==== DETAILS ====
    [HttpGet]
    public IActionResult Details(int id)
    {
        var student = _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Sessions) // vain tämä tarvitaan
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Room)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Teacher)
            .FirstOrDefault(s => s.Id == id);

        if (student == null) return NotFound();

        var viewModel = new StudentScheduleViewModel
        {
            Student = student,
            WeekStart = DateTime.Today,
            WeekEnd = DateTime.Today.AddDays(6),
            WeekOffset = 0,
            WeeklySchedule = new Dictionary<string, List<ScheduleItem>>()
        };

        return View(viewModel);
    }
    // ==== WEEKLY SCHEDULE ====
    [HttpGet]
    public IActionResult Weekly(int id, int weekOffset = 0)
    {
        var student = _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Sessions) // vain tämä
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Room)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Teacher)
            .FirstOrDefault(s => s.Id == id);

        if (student == null) return NotFound();

        // Viikon alku ja loppu
        var today = DateTime.Today.AddDays(weekOffset * 7);
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var weekStart = today.AddDays(-diff).Date;
        var weekEnd = weekStart.AddDays(6);

        // WeeklySchedule dictionary
        var weeklySchedule = new Dictionary<string, List<ScheduleItem>>();

        foreach (var enrollment in student.Enrollments)
        {
            foreach (var session in enrollment.Course.Sessions)
            {
                var item = new ScheduleItem
                {
                    Id = session.Id,
                    Name = enrollment.Course.Name,
                    Description = enrollment.Course.Description,
                    Weekday = session.WeekDay.ToString(),
                    Day_of_start = enrollment.Course.Day_of_start,
                    Day_of_end = enrollment.Course.Day_of_end,
                    Start_time = session.Time_of_start,
                    End_time = session.Time_of_end,
                    Room = enrollment.Course.Room,
                    RoomId = enrollment.Course.RoomId,
                    TeacherId = enrollment.Course.TeacherId
                };

                if (!weeklySchedule.ContainsKey(item.Weekday!))
                    weeklySchedule[item.Weekday!] = new List<ScheduleItem>();

                weeklySchedule[item.Weekday!].Add(item);
            }
        }

        // Täytä tyhjät päivät
        var allDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        foreach (var day in allDays)
        {
            if (!weeklySchedule.ContainsKey(day))
                weeklySchedule[day] = new List<ScheduleItem>();
        }

        var viewModel = new StudentScheduleViewModel
        {
            Student = student,
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            WeekOffset = weekOffset,
            WeeklySchedule = weeklySchedule
        };

        return View(viewModel);
    }

    // ==== ERROR ====
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
