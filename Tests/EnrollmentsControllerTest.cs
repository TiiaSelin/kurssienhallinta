namespace kurssienhallinta.tests;

using kurssienhallinta.Controllers;
using kurssienhallinta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class EnrollmentsControllersTest
{

    // ==== TEST LIST ====

    [Fact]
    public void Test_List_enrollments()
    {
        // Initialize in-memory database and define mock data.

        var context = DbContextFactory.CreateInMemoryDbContext();

        // Create controller instance with context defined above and mock logger.

        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        // Add student

        context.Students.Add(new Student
        {
            Id = 1,
            Student_code = "1",
            F_Name = "Alice",
            L_Name = "Wonderland",
            Birthday = new DateTime(2025, 12, 3),
            Year = 1
        });

        context.Students.Add(new Student
        {
            Id = 12,
            Student_code = "2",
            F_Name = "Bob",
            L_Name = "Builder",
            Birthday = new DateTime(1965, 12, 6),
            Year = 3
        });

        context.Students.Add(new Student
        {
            Id = 77,
            Student_code = "4",
            F_Name = "Charlie",
            L_Name = "Chaplin",
            Birthday = new DateTime(1984, 11, 2),
            Year = 2
        });

        context.SaveChanges();

        // Insert courses

        context.Courses.Add(new Course
        {
            Id = 123,
            Name = "Math",
            Day_of_start = DateTime.Now,
            Day_of_end = DateTime.Now.AddMonths(3)
        });

        context.Courses.Add(new Course
        {
            Id = 32,
            Name = "Physics",
            Day_of_start = DateTime.Now,
            Day_of_end = DateTime.Now.AddMonths(3)
        });

        context.Courses.Add(new Course
        {
            Id = 990,
            Name = "History",
            Day_of_start = DateTime.Now,
            Day_of_end = DateTime.Now.AddMonths(3)
        });

        context.SaveChanges();

        // Insert enrollments

        context.Enrollments.Add(new Enrollment
        {
            StudentId = 1,
            CourseId = 123,
            EnrollmentDate = new DateTime(1984, 5, 20)
        });

        context.Enrollments.Add(new Enrollment
        {
            StudentId = 12,
            CourseId = 32,
            EnrollmentDate = new DateTime(1953, 12, 18)
        });

        context.Enrollments.Add(new Enrollment
        {
            StudentId = 77,
            CourseId = 990,
            EnrollmentDate = new DateTime(2000, 1, 1)
        });

        context.SaveChanges();

        // Define and test that the view works.

        var result = controller.List() as ViewResult;
        Assert.IsType<ViewResult>(result);

        // Test model contains the correct information.

        var model = Assert.IsAssignableFrom<IEnumerable<Enrollment>>(result.Model);
        Assert.Equal(3, model.Count());

        Assert.Contains(model, enrollment =>
           enrollment.StudentId == 1 &&
           enrollment.CourseId == 123 &&
           enrollment.EnrollmentDate == new DateTime(1984, 5, 20)
       );

        Assert.Contains(model, enrollment =>
            enrollment.StudentId == 12 &&
            enrollment.CourseId == 32 &&
            enrollment.EnrollmentDate == new DateTime(1953, 12, 18)

        );

        Assert.Contains(model, enrollment =>
            enrollment.StudentId == 77 &&
            enrollment.CourseId == 990 &&
            enrollment.EnrollmentDate == new DateTime(2000, 1, 1)
        );
    }

    // ==== TEST ADD ENROLLMENT ====

    [Fact]
    public void Test_Add_enrollment_success()
    {
        // Arrange
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        // Must insert valid Student + Course first
        context.Students.Add(new Student
        {
            Id = 1,
            Student_code = "A",
            F_Name = "Test",
            L_Name = "Student",
            Birthday = DateTime.Now.AddYears(-20),
            Year = 1
        });

        context.Courses.Add(new Course
        {
            Id = 1,
            Name = "Math",
            Day_of_start = DateTime.Now,
            Day_of_end = DateTime.Now.AddMonths(2)
        });

        context.SaveChanges();

        // Enrollment from POST
        var enrollment = new Enrollment
        {
            StudentId = 1,
            CourseId = 1
            // Do NOT set EnrollmentDate, controller sets it
        };

        // Act
        var result = controller.Add_enrollment(enrollment);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);
        Assert.Equal(1, context.Enrollments.Count());
    }

    // ==== TEST EDIT ENROLLMENT ====

    [Fact]
    public void Test_Edit_enrollment()
    {
        // Arrange
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        context.Students.Add(new Student
        {
            Id = 1,
            Student_code = "A",
            F_Name = "Alice",
            L_Name = "Wonder",
            Birthday = DateTime.Now.AddYears(-20),
            Year = 1
        });

        context.Courses.Add(new Course
        {
            Id = 10,
            Name = "Math",
            Day_of_start = DateTime.Now,
            Day_of_end = DateTime.Now.AddMonths(3)
        });

        context.Enrollments.Add(new Enrollment
        {
            Id = 100,
            StudentId = 1,
            CourseId = 10,
            EnrollmentDate = DateTime.Now
        });

        context.SaveChanges();

        // Act
        var result = controller.Edit(100);

        // Assert
        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Enrollment>(view.Model);
        Assert.Equal(100, model.Id);
    }

    [Fact]
    public void Test_Edit_enrollment_success()
    {
        // Arrange
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        context.Students.Add(new Student
        {
            Id = 2,
            Student_code = "A",
            F_Name = "Alice",
            L_Name = "Wonder",
            Birthday = DateTime.Now.AddYears(-20),
            Year = 1
        });

        context.Courses.Add(new Course
        {
            Id = 11,
            Name = "Math",
            Day_of_start = DateTime.Now,
            Day_of_end = DateTime.Now.AddMonths(3)
        });

        context.Enrollments.Add(new Enrollment
        {
            Id = 101,
            StudentId = 1,
            CourseId = 10,
            EnrollmentDate = new DateTime(2000, 1, 1)
        });

        context.SaveChanges();

        // 🚨 Clear tracking so we can attach a new instance
        // Prevents EF from complaining about two instances with the same primary key.
        context.ChangeTracker.Clear();

        // Modify a field
        var updated = new Enrollment
        {
            Id = 101,
            StudentId = 2,
            CourseId = 11,
            EnrollmentDate = new DateTime(2025, 1, 1)
        };

        // Act
        var result = controller.Edit(updated);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);

        var saved = context.Enrollments.First(e => e.Id == 101);
        Assert.Equal(new DateTime(2025, 1, 1), saved.EnrollmentDate);
    }

    // ==== TEST DELETE ====

    [Fact]
    public void Test_Delete_Success()
    {
        // Arrange
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        context.Enrollments.Add(new Enrollment
        {
            Id = 100,
            StudentId = 1,
            CourseId = 10,
            EnrollmentDate = DateTime.Now
        });

        context.SaveChanges();

        // Act
        var result = controller.Delete_enrollment(100, true);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("List", redirect.ActionName);
        Assert.Empty(context.Enrollments);
    }

    [Fact]
    public void Test_Delete_NotFound()
    {
        // Arrange
        var context = DbContextFactory.CreateInMemoryDbContext();
        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        // Act
        var result = controller.Delete_enrollment(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
