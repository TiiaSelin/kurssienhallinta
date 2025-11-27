using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;

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
    [HttpGet]
    public IActionResult Room_details(int id)
    {
        var room = _context.Rooms
            .Include(room => room.Courses)
                 .ThenInclude(course => course.Teacher)
            .FirstOrDefault(room => room.Id == id);

        return View(room);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
