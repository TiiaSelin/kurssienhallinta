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
            if (ModelState.IsValid)
            {
                _context.Enrollments.Add(enrollment);
                _context.SaveChanges();
                return RedirectToAction("List");
            }

            // Re-populate dropdowns if validation fails
            ViewBag.Students = new SelectList(_context.Students.ToList(), "Id", "Student_code");
            ViewBag.Courses = new SelectList(_context.Courses.ToList(), "Id", "Name");
            
            return View(enrollment);
        }
    }
}