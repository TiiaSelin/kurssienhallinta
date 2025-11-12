using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class StudentsController : Controller
{
    private readonly ILogger<StudentsController> _logger;
    private readonly IConfiguration _configuration;

    public StudentsController(ILogger<StudentsController> logger, IConfiguration configuration)
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
        var opiskelijat = conn.Query<Opiskelija>(
            "SELECT opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi, syntymaaika, vuosikurssi FROM opiskelijat"
        ).ToList();

        return View(opiskelijat);
    }

    // ==== ADD STUDENT ====

    [HttpGet]
    public IActionResult AddStudent(string opiskelijatunnus)
    {

        // Luodaan yhteys Npgsql avulla hakemalla appsettings.json: "GetConnectionString", joka atm user secret

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        // Mahdollistaa dropdown menun käytön opettajistolle ja tiloille

        var opiskelijat = conn.Query<Opiskelija>(
            "SELECT opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi, syntymaaika, vuosikurssi FROM opiskelijat"
        ).ToList();

        // Luodaan viewModel, ettei tarvitse käyttää VertinDb.cs modelia kaikkeen: sekin mahdollista.
        // ViewModelista otettujen luokkien oliot (vasen) laitetaan muuttujiin (oikea)

        var viewModel = new AddStudentViewModel
        {
            Opiskelijat = opiskelijat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("AddStudent")]

    // Luodaan uusi funktio Add*, joka luokkaa IActionResult, Coren sisäinen luokka
    // Käytetään Add*ViewModel- luokkaa luomaan olio viewModeliin
    public IActionResult AddStudent(AddStudentViewModel viewModel)
    {
        // Laitetaan viewModeliin luotu olio model-muuttujaan
        // viewModel on tyyppiä Add*ViewModel, joka käyttää luokkaa *

        var model = viewModel.Opiskelija;

        // Varmistetaan, että kaikki merkkijonot menevät CAPS tietokantaan (ei pakollinen mutta ok)

        model.opiskelijatunnus = model.opiskelijatunnus?.ToUpper();
        model.opiskelijaetunimi = model.opiskelijaetunimi?.ToUpper();
        model.opiskelijasukunimi = model.opiskelijasukunimi?.ToUpper();

        // Terminaaliin tuleva log (selain log ei toimi C#)

        _logger.LogInformation("POST triggered for ID {Id}", model.opiskelijatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        // Varmistetaan ettei ainutlaatuista manuaalista ID:tä ei ole toisilla kurssitunnuksilla

        var existing = conn.QuerySingleOrDefault<Tila>(
            "SELECT * FROM opiskelijat WHERE opiskelijatunnus = @opiskelijatunnus",
            new { model.opiskelijatunnus }
            );

        if (existing != null)
        {
            ModelState.AddModelError("Opiskelija.opiskelijatunnus", "This ID already exists.");
            return View(viewModel);
        }

        conn.Execute(
            @"INSERT INTO opiskelijat
            (opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi, syntymaaika, vuosikurssi)
            VALUES
            (@opiskelijatunnus, @opiskelijaetunimi, @opiskelijasukunimi, @syntymaaika, @vuosikurssi)", model);

        return RedirectToAction("List");
    }

    // ==== MODIFY =====

    [HttpGet]
    public IActionResult ModifyStudent(string opiskelijatunnus)
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var opiskelija = conn.QuerySingleOrDefault<Opiskelija>(
            "SELECT * FROM opiskelijat WHERE opiskelijatunnus = @opiskelijatunnus",
            new { opiskelijatunnus }
        );

        if (opiskelija == null)

        {
            return NotFound($"Course with ID {opiskelijatunnus} not found.");
        }

        var opiskelijat = conn.Query<Opiskelija>(
            "SELECT opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi, syntymaaika, vuosikurssi FROM opiskelijat"
        ).ToList();

        var viewModel = new ModifyStudentViewModel
        {
            Opiskelija = opiskelija,
            Opiskelijat = opiskelijat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("ModifyStudent")]
    public IActionResult ModifyStudent(ModifyStudentViewModel viewModel)
    {

        var model = viewModel.Opiskelija;

        _logger.LogInformation("ModifyStudent POST triggered for ID {Id}", model.opiskelijatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        conn.Execute(
            @"UPDATE opiskelijat 
          SET opiskelijatunnus = @opiskelijatunnus,
              opiskelijaetunimi = @opiskelijaetunimi,
              opiskelijasukunimi = @opiskelijasukunimi,
              syntymaaika = @syntymaaika,
              vuosikurssi = @vuosikurssi
          WHERE opiskelijatunnus = @opiskelijatunnus",
            model
        );

        return RedirectToAction("List");
    }

    // ==== DELETE ====

    [HttpPost]
    public IActionResult DeleteStudent(string opiskelijatunnus)
    {
        using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DatabaseNameDB"));
        conn.Execute("DELETE FROM opiskelijat WHERE opiskelijatunnus = @opiskelijatunnus", new { opiskelijatunnus });

        return RedirectToAction("List");
    }
}
