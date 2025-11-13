using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class EnrolmentsController : Controller
{

    private readonly ILogger<EnrolmentsController> _logger;
    private readonly IConfiguration _configuration;

    public EnrolmentsController(ILogger<EnrolmentsController> logger, IConfiguration configuration)
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

        var kirjautumiset = conn.Query<Kirjautuminen>(
            "SELECT enroltunnus, opiskelijatunnus, kurssitunnus, enroldate FROM kirjautumiset"
        ).ToList();

        return View(kirjautumiset);
    }

    // ==== LIST ENROLMENTS ====

      public IActionResult ListCourseEnrolments()
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var kurssit = conn.Query<Kurssi>(
            "SELECT kurssitunnus FROM kurssit"
        ).ToList();

        
        var opiskelijat = conn.Query<Opiskelija>(
            "SELECT opiskelijatunnus FROM opiskelijat"
        ).ToList();

         var viewModel = new KurssinOpiskelijatViewModel
        {
            Kurssit = kurssit,
            Opiskelijat = opiskelijat
        };

        return View(viewModel);
    }
    

    // ==== ADD ====

    [HttpGet]
    public IActionResult AddEnrolment()
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var kurssit = conn.Query<Kurssi>(
            "SELECT kurssitunnus, kurssinimi FROM kurssit"
        ).ToList();

        var opiskelijat = conn.Query<Opiskelija>(
            "SELECT opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi FROM opiskelijat"
        ).ToList();

        var viewModel = new AddEnrolmentViewModel
        {
            Kurssit = kurssit,
            Opiskelijat = opiskelijat
        };

        return View(viewModel);
    }

    [HttpPost]
    [ActionName("AddEnrolment")]
    public IActionResult AddEnrolment(AddEnrolmentViewModel viewModel)
    {
        var model = viewModel.Kirjautuminen;

        model.enroldate = DateTime.Today;

        _logger.LogInformation("POST triggered for ID {Id}", model.enroltunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        // Varmista ettei ole jo kirjautunut kurssille

        var existing = conn.QueryFirstOrDefault<Kirjautuminen>(
        "SELECT * FROM kirjautumiset WHERE opiskelijatunnus = @opiskelijatunnus AND kurssitunnus = @kurssitunnus",
        new { model.opiskelijatunnus, model.kurssitunnus }
    );

        if (existing != null)
        {
            ModelState.AddModelError("Kirjautuminen.kurssitunnus", "This student is already enrolled in this course.");

            viewModel.Kurssit = conn.Query<Kurssi>(
                "SELECT kurssitunnus, kurssinimi FROM kurssit"
            ).ToList();
            viewModel.Opiskelijat = conn.Query<Opiskelija>(
                "SELECT opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi FROM opiskelijat"
            ).ToList();

            return View(viewModel);
        }

        conn.Execute(
            @"INSERT INTO kirjautumiset 
            (opiskelijatunnus, kurssitunnus, enroldate)
          VALUES 
          (@opiskelijatunnus, @kurssitunnus, @enroldate)",
            model);

        return RedirectToAction("List");
    }

    // ==== MODIFY =====

    [HttpGet]
    public IActionResult ModifyEnrolment(int enroltunnus)
    {
        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        var kirjautuminen = conn.QuerySingle<Kirjautuminen>(
            "SELECT * FROM kirjautumiset WHERE enroltunnus = @enroltunnus",
            new { enroltunnus }
        );

        // Null check 

        if (kirjautuminen == null)

        {
            return NotFound($"Data with ID {kirjautuminen} not found.");
        }

        var kurssit = conn.Query<Kurssi>(
             "SELECT kurssitunnus, kurssinimi FROM kurssit"
         ).ToList();

        var opiskelijat = conn.Query<Opiskelija>(
            "SELECT opiskelijatunnus, opiskelijaetunimi, opiskelijasukunimi FROM opiskelijat"
        ).ToList();



        var viewModel = new ModifyEnrolmentViewModel
        {
            Kirjautuminen = kirjautuminen,
            Kurssit = kurssit,
            Opiskelijat = opiskelijat
        };



        return View(viewModel);


    }

    [HttpPost]
    [ActionName("ModifyEnrolment")]
    public IActionResult ModifyEnrolment(ModifyEnrolmentViewModel viewModel)
    {
        var model = viewModel.Kirjautuminen;

        _logger.LogInformation("POST triggered for ID {Id}", model.enroltunnus);

        string connectionString = _configuration.GetConnectionString("DatabaseNameDB")
                                  ?? throw new InvalidOperationException("Connection string not found.");

        using var conn = new NpgsqlConnection(connectionString);

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Kurssi model was null. Check your form field names.");
        }

        conn.Execute(
            @"UPDATE kirjautumiset 
          SET 
              opiskelijatunnus = @opiskelijatunnus,
              kurssitunnus = @kurssitunnus,
              enroldate = @enroldate
          WHERE enroltunnus = @enroltunnus",
            model
        );

        return RedirectToAction("List");
    }



    // ==== DELETE ====

    [HttpPost]
    public IActionResult DeleteEnrolment(int enroltunnus)
    {
        using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DatabaseNameDB"));
        conn.Execute("DELETE FROM kirjautumiset WHERE enroltunnus = @enroltunnus", new { enroltunnus });

        return RedirectToAction("List");
    }
}
