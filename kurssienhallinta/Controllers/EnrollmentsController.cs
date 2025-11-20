using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kurssienhallinta.Models;

namespace kurssienhallinta.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(AppDbContext context, ILogger<EnrollmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Enrollments/List
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

        // GET: Enrollments/Add_enrollment
        [HttpGet]
        public IActionResult Add_enrollment()
        {
            // Populate dropdown lists for the form
            ViewBag.Students = new SelectList(_context.Students.ToList(), "Id", "Student_code");
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");

            return View();
        }

        // POST: Enrollments/Add_enrollment
        [HttpPost]
        public IActionResult Add_enrollment(Enrollment enrollment)
        {
            ViewBag.Students = new SelectList(_context.Students.ToList(), "Id", "Student_code");
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");

            if (ModelState.IsValid)
            {
                enrollment.EnrollmentDate = DateTime.SpecifyKind(
                    enrollment.EnrollmentDate,
                    DateTimeKind.Unspecified
                );
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
    }
}