using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;

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


    [HttpGet]
    public IActionResult List()
    {
        var students = _context.Students.ToList();
        return View(students);
    }


    [HttpGet]
    public IActionResult Add_student()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Add_student(Student student)
    {
        if (ModelState.IsValid)
        {

            student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        return View(student);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var student = _context.Students.FirstOrDefault(s => s.Id == id);
        if (student == null)
            return NotFound();

        return View(student);
    }

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (!ModelState.IsValid)
            return View(student);

        student.Birthday = DateTime.SpecifyKind(student.Birthday, DateTimeKind.Utc);

        _context.Students.Update(student);
        _context.SaveChanges();
        return RedirectToAction("List");
    }



    [HttpGet]
    public IActionResult Details(int id)
    {
        var student = _context.Students.FirstOrDefault(s => s.Id == id);
        if (student == null)
            return NotFound();

        var enrollments = _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == id)
            .ToList();

        ViewBag.Enrollments = enrollments;

        return View(student);
    }



    [HttpPost]
    public IActionResult Delete(int id)
    {
        var student = _context.Students.Find(id);

        if (student == null)
            return NotFound();

        _context.Students.Remove(student);
        _context.SaveChanges();

        return RedirectToAction("List");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
