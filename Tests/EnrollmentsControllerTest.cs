namespace kurssienhallinta.tests;

using kurssienhallinta.Controllers;
using kurssienhallinta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class EnrollmentsControllersTest
{
    [Fact]
    public void Test_List_enrollments()
    {
        // Initialize in-memory database and define mock data.

        var context = DbContextFactory.CreateInMemoryDbContext();

        // Create controller instance with context defined above and mock logger.

        var logger = NullLogger<EnrollmentsController>.Instance;
        var scheduleService = new ScheduleService();
        var controller = new EnrollmentsController(context, logger, scheduleService);

        // Insert students
        context.Students.AddRange(
            new Student
            {
                Id = 1,
                Student_code = "1",
                F_Name = "Alice",
                L_Name = "Wonderland",
                Birthday = new DateTime(2025, 12, 3),
                Year = 1
            },

            new Student
            {
                Id = 12,
                Student_code = "2",
                F_Name = "Bob",
                L_Name = "Builder",
                Birthday = new DateTime(1965, 12, 6),
                Year = 3
            },

            new Student
            {
                Id = 77,
                Student_code = "4",
                F_Name = "Charlie",
                L_Name = "Chaplin",
                Birthday = new DateTime(1984, 11, 2),
                Year = 2
            }
        );

        // Insert courses
        context.Courses.AddRange(
            new Course
            {
                Id = 123,
                Name = "Math",
                Day_of_start = DateTime.Now,
                Day_of_end = DateTime.Now.AddMonths(3)
            },

            new Course
            {
                Id = 32,
                Name = "Physics",
                Day_of_start = DateTime.Now,
                Day_of_end = DateTime.Now.AddMonths(3)
            },

            new Course
            {
                Id = 990,
                Name = "History",
                Day_of_start = DateTime.Now,
                Day_of_end = DateTime.Now.AddMonths(3)
            }
        );

        context.SaveChanges();


        context.Enrollments.Add(new Enrollment
        {
            StudentId = 1,
            CourseId = 123,
            EnrollmentDate = new DateTime(1984, 5, 20)
        });
        context.SaveChanges();

        context.Enrollments.Add(new Enrollment
        {
            StudentId = 12,
            CourseId = 32,
            EnrollmentDate = new DateTime(1953, 12, 18)

        });
        context.SaveChanges();

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
}
