using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers;

public class RoomsController : Controller
{
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(ILogger<RoomsController> logger)
    {
        _logger = logger;
    }

    public IActionResult List()
    {
        return View();
    }
    public IActionResult Add_room()
    {
        return View();
    }
}
