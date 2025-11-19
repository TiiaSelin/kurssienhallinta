using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult List()
    {
        return View();
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
            return RedirectToAction("List");
        }
        return View(room);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
