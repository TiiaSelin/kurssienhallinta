using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;
using kurssienhallinta.Services;

namespace kurssienhallinta.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EnrollmentsController> _logger;
        private readonly ScheduleService _scheduleService;

        public EnrollmentsController(AppDbContext context, ILogger<EnrollmentsController> logger, ScheduleService scheduleService)
        {
            _context = context;
            _logger = logger;
            _scheduleService = scheduleService;
        }

        // ==== LIST ====
        [HttpGet]
        public IActionResult List()
        {
            // Include() loads related Student and Course data (JOIN in SQL)
            var enrollments = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToList();

            return View(enrollments);
        }

        // ==== ADD ====

        [HttpGet]
        public IActionResult Add_enrollment()
        {
            // Populate dropdown lists for the form
            ViewBag.Students = new SelectList(
            _context.Students
            .Select(student => new
        {
            Id = student.Id,
            FullName = $"{student.Student_code}, {student.F_Name} {student.L_Name}"
        }).ToList(), "Id", "FullName");
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Add_enrollment(Enrollment enrollment)
        {
            ViewBag.Students = new SelectList(_context.Students.ToList(), "Id", "Student_code");
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {
                enrollment.EnrollmentDate = DateTime.Now; // Tämän kanssa ei tarvitse erikseen laittaa formissa ilmoittautumispäivämäärää
                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();
                return RedirectToAction("List");
            }

            return View(enrollment);
        }

        // === EDIT ====

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var enrollment = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.Id == id);


            if (enrollment == null)
            {
                return NotFound();
            }

            // Populate dropdown lists for the form
            ViewBag.Students = new SelectList(_context.Students.ToList(), "Id", "Student_code");
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");

            return View(enrollment);
        }

        [HttpPost]
        public IActionResult Edit(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Enrollments.Update(enrollment);
                _context.SaveChanges();
                return RedirectToAction("List");
            }

            return View(enrollment);
        }

        // ==== DELETE ====

        [HttpPost]
        public IActionResult Delete_enrollment(int id, bool confirm = false)
        {
            var selected_enrollment = _context.Enrollments.FirstOrDefault(enrollment => enrollment.Id == id);

            if (selected_enrollment == null)
            {
                return NotFound();
            }
            _context.Enrollments.Remove(selected_enrollment);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}