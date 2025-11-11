using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class RoomsController : Controller
{
    private readonly ILogger<CoursesController> _logger;
    private readonly IConfiguration _configuration;

    public RoomsController(ILogger<CoursesController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    // ==== LIST =====
    public IActionResult List()
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);
        var tilat = conn.Query<Tila>(
            "SELECT tilatunnus, tilanimi, tilakapasiteetti FROM tilat"
        ).ToList();

        return View(tilat);
    }
    

}
