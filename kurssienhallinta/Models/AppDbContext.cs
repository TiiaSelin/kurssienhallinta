using kurssienhallinta.Controllers;
using Microsoft.EntityFrameworkCore;

namespace kurssienhallinta.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseSession> CourseSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Course>()
            .HasOne(course => course.Teacher)
            .WithMany(teacher => teacher.Courses)
            .HasForeignKey(course => course.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
            .HasOne(course => course.Room)
            .WithMany(c => c.Courses)
            .HasForeignKey(course => course.RoomId)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
            .Property(e => e.EnrollmentDate)
            .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<CourseSession>()
            .ToTable("CourseSessions")
            .HasOne(c => c.Course)
            .WithMany(s => s.Sessions)
            .HasForeignKey(c => c.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Name = "C# kurssi.",
                    Description = "Näe terävästi ja opettele hianoo kieltä.",
                    Day_of_start = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
                    Day_of_end = DateTime.SpecifyKind(new DateTime(2025, 2, 1), DateTimeKind.Utc),
                    TeacherId = 1,
                    RoomId = 1
                },

                new Course
                {
                    Id = 2,
                    Name = "Sienestyskurssi",
                    Description = "Opetellaan tunnistamaan sieniä, syömään sieniä, arvostamaan sieniä, rihmastoitumaan.",
                    Day_of_start = DateTime.SpecifyKind(new DateTime(1900, 1, 1), DateTimeKind.Utc),
                    Day_of_end = DateTime.SpecifyKind(new DateTime(9001, 1, 1), DateTimeKind.Utc),
                    TeacherId = 1,
                    RoomId = 1
                }
            );

            modelBuilder.Entity<Teacher>().HasData(
               new Teacher
               {
                   Id = 1,
                   Teacher_code = "A10",
                   First_name = "Ville",
                   Last_name = "Virtanen",
                   Subject = "Matematiikka"
               },
               new Teacher
               {
                   Id = 2,
                   Teacher_code = "B10",
                   First_name = "Anna",
                   Last_name = "Korhonen",
                   Subject = "Web-ohjelmointi"
               }

           );

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Opetus Tila_0",
                    Capacity = 100,
                    Room_code = "OpT0"
                },
                new Room
                {
                    Id = 2,
                    Name = "Kapselihotelli",
                    Capacity = 1,
                    Room_code = "Kp0"
                }
            );



            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Student_code = "SM0",
                    F_Name = "Sieni",
                    L_Name = "Mies",
                    Birthday = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc),
                    Year = 1
                },
                new Student
                {
                    Id = 2,
                    Student_code = "FG0",
                    F_Name = "Fun",
                    L_Name = "Guy",
                    Birthday = DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
                    Year = 2
                }
            );

            modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                Id = 1,
                StudentId = 1,  // Sieni Mies
                CourseId = 2,   // Sienestyskurssi
                EnrollmentDate = DateTime.SpecifyKind(new DateTime(2025, 1, 15), DateTimeKind.Utc)
            },
                new Enrollment
                {
                    Id = 2,
                    StudentId = 2,  // Fun Guy
                    CourseId = 1,   // C# kurssi
                    EnrollmentDate = DateTime.SpecifyKind(new DateTime(2025, 1, 10), DateTimeKind.Utc)
                }
            );

        }

    }
}

