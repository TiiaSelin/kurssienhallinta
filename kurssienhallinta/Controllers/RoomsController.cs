using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class RoomsController : Controller
{
    private readonly ILogger<RoomsController> _logger;
    private readonly IConfiguration _configuration;

    public RoomsController(ILogger<RoomsController> logger, IConfiguration configuration)
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

    // ==== ADD ROOM ====

    [HttpGet]
    public IActionResult AddRoom(string tilatunnus)
    {

        // Luodaan yhteys Npgsql avulla hakemalla appsettings.json: "GetConnectionString", joka atm user secret

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        // Mahdollistaa dropdown menun käytön opettajistolle ja tiloille

        var tilat = conn.Query<Tila>(
            "SELECT tilatunnus, tilanimi FROM tilat"
        ).ToList();

        // Luodaan viewModel, ettei tarvitse käyttää VertinDb.cs modelia kaikkeen: sekin mahdollista.
        // ViewModelista otettujen luokkien oliot (vasen) laitetaan muuttujiin (oikea)

        var viewModel = new AddRoomViewModel
        {
            Tilat = tilat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("AddRoom")]

    // Luodaan uusi funktio Add*, joka luokkaa IActionResult, Coren sisäinen luokka
    // Käytetään Add*ViewModel- luokkaa luomaan olio viewModeliin
    public IActionResult AddRoom(AddRoomViewModel viewModel)
    {
        // Laitetaan viewModeliin luotu olio model-muuttujaan
        // viewModel on tyyppiä Add*ViewModel, joka käyttää luokkaa *

        var model = viewModel.Tila;

        // Varmistetaan, että kaikki merkkijonot menevät CAPS tietokantaan (ei pakollinen mutta ok)

        model.tilatunnus = model.tilatunnus?.ToUpper();
        model.tilanimi = model.tilanimi?.ToUpper();

        // Terminaaliin tuleva log (selain log ei toimi C#)

        _logger.LogInformation("AddCourse POST triggered for ID {Id}", model.tilatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        // Varmistetaan ettei ainutlaatuista manuaalista ID:tä ei ole toisilla kurssitunnuksilla

        var existing = conn.QuerySingleOrDefault<Tila>(
            "SELECT * FROM tilat WHERE tilatunnus = @tilatunnus",
            new { model.tilatunnus }
            );

        if (existing != null)
        {
            ModelState.AddModelError("Tila.tilatunnus", "This ID already exists.");
            return View(viewModel);
        }

        conn.Execute(
            @"INSERT INTO tilat
            (tilatunnus, tilanimi, tilakapasiteetti)
            VALUES
            (@tilatunnus, @tilanimi, @tilakapasiteetti)", model);

        return RedirectToAction("List");
    }

    // ==== MODIFY =====

    [HttpGet]
    public IActionResult ModifyRoom(string tilatunnus)
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var tila = conn.QuerySingleOrDefault<Tila>(
            "SELECT * FROM tilat WHERE tilatunnus = @tilatunnus",
            new { tilatunnus }
        );

        if (tila == null)

        {
            return NotFound($"Course with ID {tilatunnus} not found.");
        }

        var tilat = conn.Query<Tila>(
            "SELECT tilatunnus, tilanimi, tilakapasiteetti FROM tilat"
        ).ToList();

        var viewModel = new ModifyRoomViewModel
        {
            Tila = tila,
            Tilat = tilat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("ModifyRoom")]
    public IActionResult ModifyRoom(ModifyRoomViewModel viewModel)
    {

        var model = viewModel.Tila;

        _logger.LogInformation("ModifyRoom POST triggered for ID {Id}", model.tilatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        conn.Execute(
            @"UPDATE tilat 
          SET tilatunnus = @tilatunnus,
              tilanimi = @tilanimi,
              tilakapasiteetti = @tilakapasiteetti 
          WHERE tilatunnus = @tilatunnus",
            model
        );

        return RedirectToAction("List");
    }

    // ==== DELETE ====

    [HttpPost]
    public IActionResult DeleteRoom(string tilatunnus)
    {
        using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DatabaseNameDB"));
        conn.Execute("DELETE FROM tilat WHERE tilatunnus = @tilatunnus", new { tilatunnus });

        return RedirectToAction("List");
    }   
}
