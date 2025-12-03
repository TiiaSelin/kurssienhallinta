using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

namespace kurssienhallinta.Controllers;

public class StudentsController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(AppDbContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ==== LIST ====
    [HttpGet]
    public IActionResult List()
    {
        var students = _context.Students.ToList();
        return View(students);
    }

    // ==== ADD ====
    [HttpGet]
    public IActionResult Add_student() => View();

    [HttpPost]
    public IActionResult Add_student(Student student)
    {
        if (!ModelState.IsValid) return View(student);

        student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

        _context.Students.Add(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    // ==== EDIT ====
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return NotFound();
        return View(student);
    }

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (!ModelState.IsValid) return View(student);

        student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

        _context.Students.Update(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    // ==== DELETE ====
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var student = _context.Students.Find(id);
        if (student == null) return NotFound();

        _context.Students.Remove(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    // ==== DETAILS ====
    [HttpGet]
    public async Task<IActionResult> Details(int id, int weekOffset = 0)
    {
        // Hae opiskelija
        var student = await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Sessions)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Room)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Teacher)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
            return NotFound();

        // Luo ScheduleService-instanse (voit tehdä tämän myös dependency injectionilla)
        var scheduleService = new ScheduleService();

        // Luo ViewModel opiskelijan aikataululle
        var viewModel = scheduleService.BuildStudentSchedule(student, weekOffset);

        return View(viewModel);
    }



    // ==== ERROR ====
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
