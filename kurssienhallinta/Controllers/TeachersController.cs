using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class TeachersController : Controller
{
    private readonly ILogger<TeachersController> _logger;
    private readonly IConfiguration _configuration;

    public TeachersController(ILogger<TeachersController> logger, IConfiguration configuration)
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
        var opettajat = conn.Query<Opettaja>(
            "SELECT opettajatunnus, opettajaetunimi, opettajasukunimi, opettajaaine FROM opettajat"
        ).ToList();

        return View(opettajat);
    }

    // ==== ADD ====

    [HttpGet]
    public IActionResult AddTeacher(string opettajatunnus)
    {

        // Luodaan yhteys Npgsql avulla hakemalla appsettings.json: "GetConnectionString", joka atm user secret

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        // Mahdollistaa dropdown menun käytön opettajistolle ja tiloille

        var opettajat = conn.Query<Opettaja>(
             "SELECT opettajatunnus, opettajaetunimi, opettajasukunimi, opettajaaine FROM opettajat"
        ).ToList();

        // Luodaan viewModel, ettei tarvitse käyttää VertinDb.cs modelia kaikkeen: sekin mahdollista.
        // ViewModelista otettujen luokkien oliot (vasen) laitetaan muuttujiin (oikea)

        var viewModel = new AddTeacherViewModel
        {
            Opettajat = opettajat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("AddTeacher")]

    // Luodaan uusi funktio Add*, joka luokkaa IActionResult, Coren sisäinen luokka
    // Käytetään Add*ViewModel- luokkaa luomaan olio viewModeliin
    public IActionResult AddTeacher(AddTeacherViewModel viewModel)
    {
        // Laitetaan viewModeliin luotu olio model-muuttujaan
        // viewModel on tyyppiä Add*ViewModel, joka käyttää luokkaa *

        var model = viewModel.Opettaja;

        // Varmistetaan, että kaikki merkkijonot menevät CAPS tietokantaan (ei pakollinen mutta ok)

        model.opettajatunnus = model.opettajatunnus?.ToUpper();
        model.opettajaetunimi = model.opettajaetunimi?.ToUpper();
        model.opettajasukunimi = model.opettajasukunimi?.ToUpper();

        // Terminaaliin tuleva log (selain log ei toimi C#)

        _logger.LogInformation("POST triggered for ID {Id}", model.opettajatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        // Varmistetaan ettei ainutlaatuista manuaalista ID:tä ei ole toisilla kurssitunnuksilla

        var existing = conn.QuerySingleOrDefault<Opettaja>(
            "SELECT * FROM opettajat WHERE opettajatunnus = @opettajatunnus",
            new { model.opettajatunnus }
            );

        if (existing != null)
        {
            ModelState.AddModelError("Opettaja.opettajatunnus", "This ID already exists.");
            return View(viewModel);
        }

        conn.Execute(
            @"INSERT INTO opettajat
            (opettajatunnus, opettajaetunimi, opettajasukunimi, opettajaaine)
            VALUES
            (@opettajatunnus, @opettajaetunimi, @opettajasukunimi, @opettajaaine)", model);

        return RedirectToAction("List");
    }

}

 