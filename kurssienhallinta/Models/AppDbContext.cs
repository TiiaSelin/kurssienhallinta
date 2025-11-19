using Microsoft.EntityFrameworkCore;

namespace kurssienhallinta.Models
{
    public class AppDbContext : DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Student> Students {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Name = "C# kurssi.",
                    Description = "Näe terävästi ja opettele hianoo kieltä.",
                    Day_of_start = DateTime.SpecifyKind(new DateTime(2025, 1, 1), DateTimeKind.Utc),
                    Day_of_end = DateTime.SpecifyKind(new DateTime(2025, 2, 1), DateTimeKind.Utc)
                },

                new Course
                {
                    Id = 2,
                    Name = "Sienestyskurssi",
                    Description = "Opetellaan tunnistamaan sieniä, syömään sieniä, arvostamaan sieniä, rihmastoitumaan.",
                    Day_of_start = DateTime.SpecifyKind(new DateTime(1900, 1, 1), DateTimeKind.Utc),
                    Day_of_end = DateTime.SpecifyKind(new DateTime(9001, 1, 1), DateTimeKind.Utc)
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
                    Year= 2
                }
            );
        }

    }
}

