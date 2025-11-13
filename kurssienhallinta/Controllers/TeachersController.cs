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

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var opettajat = conn.Query<Opettaja>(
             "SELECT opettajatunnus, opettajaetunimi, opettajasukunimi, opettajaaine FROM opettajat"
        ).ToList();

        var viewModel = new AddTeacherViewModel
        {
            Opettajat = opettajat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("AddTeacher")]
    public IActionResult AddTeacher(AddTeacherViewModel viewModel)
    {
        var model = viewModel.Opettaja;

        // Varmistetaan, että kaikki merkkijonot menevät CAPS tietokantaan (ei pakollinen)

        model.opettajatunnus = model.opettajatunnus?.ToUpper();
        model.opettajaetunimi = model.opettajaetunimi?.ToUpper();
        model.opettajasukunimi = model.opettajasukunimi?.ToUpper();

        // Terminaali log

        _logger.LogInformation("POST triggered for ID {Id}", model.opettajatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        // Varmistetaan ettei ainutlaatuista manuaalista ID:tä ei ole toisilla kurssitunnuksilla

        var existing = conn.QuerySingle<Opettaja>(
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

    // ==== MODIFY =====

    [HttpGet]
    public IActionResult ModifyTeacher(string opettajatunnus)
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var opettaja = conn.QuerySingle<Opettaja>(
            "SELECT * FROM opettajat WHERE opettajatunnus = @opettajatunnus",
            new { opettajatunnus }
        );

        // Null check 

        if (opettaja == null)

        {
            return NotFound($"Data with ID {opettajatunnus} not found.");
        }

        var opettajat = conn.Query<Opettaja>(
            "SELECT opettajatunnus, opettajaetunimi, opettajasukunimi, opettajaaine FROM opettajat"
        ).ToList();

        var viewModel = new ModifyTeacherViewModel
        {
            Opettaja = opettaja,
            Opettajat = opettajat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("ModifyTeacher")]
    public IActionResult ModifyTeacher(ModifyTeacherViewModel viewModel)
    {
        var model = viewModel.Opettaja;

        _logger.LogInformation("POST triggered for ID {Id}", model.opettajatunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        // Nullcheck
        
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Model was null. Check your form field names.");
        }

        conn.Execute(
            @"UPDATE opettajat 
          SET opettajatunnus = @opettajatunnus,
              opettajaetunimi = @opettajaetunimi,
              opettajasukunimi = @opettajasukunimi,
              opettajaaine = @opettajaaine
          WHERE opettajatunnus = @opettajatunnus",
            model
        );

        return RedirectToAction("List");
    }



}

