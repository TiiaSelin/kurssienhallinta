using Microsoft.EntityFrameworkCore;

namespace kurssienhallinta.Models
{
    public class AppDbContext : DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Room> Rooms { get; set; }

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
        }

    }
}

