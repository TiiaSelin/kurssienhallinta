using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class RoomsController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(AppDbContext context, ILogger<RoomsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult List_rooms()
    {
        var rooms = _context.Rooms.ToList();
        return View(rooms);
    }
    [HttpGet]
    public IActionResult Add_room()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Add_room(Room room)
    {
        if (ModelState.IsValid)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return RedirectToAction("List_rooms");
        }
        return View(room);
    }
    [HttpGet]
    public IActionResult Edit_room(int id)
    {
        var selected_room = _context.Rooms.FirstOrDefault(room => room.Id == id);

        if (selected_room == null)
        {
            return NotFound();
        }
        return View(selected_room);
    }
    [HttpPost]
    public IActionResult Edit_room(Room room)
    {
        if (!ModelState.IsValid)
        {
            return View(room);
        }

        _context.Rooms.Update(room);
        _context.SaveChanges();
        return RedirectToAction("List_rooms");
    }
    [HttpGet]
    public IActionResult Delete_room(int id)
    {
        var selected_room = _context.Rooms.FirstOrDefault(room => room.Id == id);

        if (selected_room == null)
        {
            return NotFound();
        }
        return View(selected_room);
    }
    [HttpPost]
    public IActionResult Delete_room(int id, bool confirm = false)
    {
        var selected_room = _context.Rooms.FirstOrDefault(room => room.Id == id);

        if (selected_room == null)
        {
            return NotFound();
        }
        _context.Rooms.Remove(selected_room);
        _context.SaveChanges();
        return RedirectToAction("List_rooms");
    }
    /*
    [HttpGet]
    public IActionResult Room_details(int id)
    {
        var room = _context.Rooms
            .Include(room => room.Courses)
                .ThenInclude(course => course.Teacher)
            .Include(room => room.Courses)
                .ThenInclude(course => course.Enrollments)
            .FirstOrDefault(room => room.Id == id);

        return View(room);
    }
    */
    [HttpGet]
    public IActionResult Room_details(int id, int weekOffset = 0)
    {
        var room = _context.Rooms // Vaihtuu
            .Include(room => room.Courses)
                .ThenInclude(course => course.Sessions)
            .Include(room => room.Courses)
                .ThenInclude(course => course.Teacher)
            .FirstOrDefault(room => room.Id == id);

        if (room == null) // Vaihtuu
            return NotFound();

        var today = DateTime.Now;
        var daysOfWeek = (int)today.DayOfWeek;
        var mondayThisWeek = today.AddDays(-(daysOfWeek == 0 ? 6 : daysOfWeek - 1)).Date;
        var weekStart = mondayThisWeek.AddDays(weekOffset * 7);
        var weekEnd = weekStart.AddDays(6);

        var sessions = room.Courses // Vaihtuu
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
            Weekday = cs.WeekDay switch
                {
                    DayOfWeek.Monday    => "Maanantai.",
                    DayOfWeek.Tuesday   => "Tiistai.",
                    DayOfWeek.Wednesday => "Keskiviikko.",
                    DayOfWeek.Thursday  => "Torstai.",
                    DayOfWeek.Friday    => "Perjantai.",
                    _ => ""  
                },
            Room = cs.Course.Room
        }).ToList();

        var weeklySchedule = scheduleItems
            .GroupBy(si => si.Weekday)
            .ToDictionary(g => g.Key, g => g.OrderBy(si => si.Start_time).ToList());

        var allDays = new[] { "Maanantai.", "Tiistai.", "Keskiviikko.", "Torstai.", "Perjantai." };
        foreach (var day in allDays)
        {
            if (!weeklySchedule.ContainsKey(day))
                weeklySchedule[day] = new List<ScheduleItem>();
        }

        var viewModel = new RoomScheduleViewModel // Vaihtuu
        {
            Room = room, // Vaihtuu
            WeeklySchedule = weeklySchedule,
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            WeekOffset = weekOffset
        };

        return View(viewModel);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
