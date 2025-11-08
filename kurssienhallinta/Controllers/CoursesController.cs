using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using VertinDb.Models;
using VertinDb.Models.ViewModels;

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

    // ==== LIST =====
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

    // ==== MODIFY =====

    [HttpGet]
    public IActionResult ModifyCourse(string kurssitunnus)
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var kurssi = conn.QuerySingleOrDefault<Kurssi>(
            "SELECT * FROM kurssit WHERE kurssitunnus = @kurssitunnus",
            new { kurssitunnus }
        );

        if (kurssi == null)
        {
            return NotFound($"Course with ID {kurssitunnus} not found.");
        }

        var opettajat = conn.Query<Opettaja>(
            "SELECT opettajatunnus, opettajaetunimi, opettajasukunimi FROM opettajat"
        ).ToList();

        var tilat = conn.Query<Tila>(
            "SELECT tilatunnus, tilanimi FROM tilat"
        ).ToList();

        var viewModel = new ModifyCourseViewModel
        {
            Kurssi = kurssi!,
            Opettajat = opettajat,
            Tilat = tilat
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult ModifyCourse(Kurssi model)
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        conn.Execute(
            @"UPDATE kurssit 
          SET kurssinimi = @kurssinimi, 
              kurssikuvaus = @kurssikuvaus,
              kurssialoituspaiva = @kurssialoituspaiva,
              kurssilopetuspaiva = @kurssilopetuspaiva,
              opettajatunnus = @opettajatunnus,
              tilatunnus = @tilatunnus
          WHERE kurssitunnus = @kurssitunnus",
            model
        );

        return RedirectToAction("List");
    }
}
