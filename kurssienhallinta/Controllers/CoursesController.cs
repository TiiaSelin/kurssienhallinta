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

    [HttpGet]
    public IActionResult AddCourse(string kurssitunnus)
    {

        // Luodaan yhteys Npgsql avulla hakemalla appsettings.json: "GetConnectionString", joka atm user secret

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        // Mahdollistaa dropdown menun käytön opettajistolle ja tiloille

        var opettajat = conn.Query<Opettaja>(
            "SELECT opettajatunnus, opettajaetunimi, opettajasukunimi FROM opettajat"
        ).ToList();

        var tilat = conn.Query<Tila>(
            "SELECT tilatunnus, tilanimi FROM tilat"
        ).ToList();

        // Luodaan viewModel, ettei tarvitse käyttää VertinDb.cs modelia kaikkeen: sekin mahdollista.
        // ViewModelista otettujen luokkien oliot (vasen) laitetaan muuttujiin (oikea)

        var viewModel = new AddCourseViewModel
        {
            Kurssi = new Kurssi(), // ViewModel -> VertinDb -> Kurssi-luokkaa käyetään luomaan uusi kurssi
            Opettajat = opettajat,
            Tilat = tilat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("AddCourse")]

    // Luodaan uusi funktio AddCourse, joka luokkaa IActionResult, Coren sisäinen luokka
    // Käytetään AddCourseViewModel- luokkaa luomaan olio viewModeliin
    public IActionResult AddCourse(AddCourseViewModel viewModel)
    {

        // Laitetaan viewModeliin luotu olio model-muuttujaan
        // viewModel on tyyppiä AddCourseViewModel, joka käyttää luokkaa Kurssi (VertinDb.cs)

        var model = viewModel.Kurssi;

        // Varmistetaan, että kaikki merkkijonot menevät CAPS tietokantaan (ei pakollinen mutta ok)

        model.kurssitunnus = model.kurssitunnus?.ToUpper();
        model.kurssinimi = model.kurssinimi?.ToUpper();
        model.kurssikuvaus = model.kurssikuvaus?.ToUpper();
        model.tilatunnus = model.tilatunnus?.ToUpper();


        // Terminaaliin tuleva log (selain log ei toimi C#)

        _logger.LogInformation("AddCourse POST triggered for ID {Id}", model.kurssitunnus);


        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        // Varmistetaan ettei ainutlaatuista manuaalista ID:tä ei ole toisilla kurssitunnuksilla

        var existing = conn.QuerySingleOrDefault<Kurssi>(
            "SELECT * FROM kurssit WHERE kurssitunnus = @kurssitunnus",
            new { model.kurssitunnus }
            );

        if (existing != null)
        {
            ModelState.AddModelError("Kurssi.kurssitunnus", "This ID already exists.");
            return View(viewModel);
        }

        conn.Execute(
            @"INSERT INTO kurssit
            (kurssitunnus, kurssinimi, kurssikuvaus, kurssialoituspaiva, kurssilopetuspaiva, opettajatunnus, tilatunnus)
            VALUES
            (@kurssitunnus, @kurssinimi, @kurssikuvaus, @kurssialoituspaiva, @kurssilopetuspaiva, @opettajatunnus, @tilatunnus)", model);

        return RedirectToAction("List");
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
    [ActionName("ModifyCourse")]
    public IActionResult ModifyCourse(ModifyCourseViewModel viewModel)
    {

        var model = viewModel.Kurssi;

        _logger.LogInformation("ModifyCourse POST triggered for ID {Id}", model.kurssitunnus);

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

    // ==== DELETE ====

    [HttpPost]
    public IActionResult DeleteCourse(string kurssitunnus)
    {
        using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DatabaseNameDB"));
        conn.Execute("DELETE FROM kurssit WHERE kurssitunnus = @kurssitunnus", new { kurssitunnus });

        return RedirectToAction("List");
    }
}