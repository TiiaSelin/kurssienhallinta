using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using VertinDb.Models;

namespace kurssienhallinta.Controllers;

public class CoursesController : Controller
{
    private readonly ILogger<CoursesController> _logger;
    private readonly IConfiguration _configuration;

    public CoursesController(ILogger<CoursesController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult List()
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);
        var kurssit = conn.Query<Kurssi>(
            "SELECT kurssitunnus, kurssinimi, kurssikuvaus, kurssialoituspaiva, kurssilopetuspaiva, opettajatunnus, tilatunnus FROM kurssit"
        ).ToList();

        return View(kurssit);
    }

    public IActionResult AddCourse()
    {
        return View();
    }

    public IActionResult ModifyCourse()
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var kurssit = conn.Query<Kurssi>(
            "SELECT kurssitunnus, kurssinimi, kurssikuvaus, kurssialoituspaiva, kurssilopetuspaiva, opettajatunnus, tilatunnus FROM kurssit"
        ).ToList();

        // Pass the list to the view
        return View(kurssit);
    }
}
