using System;

namespace kurssienhallinta.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string? Name { get; set;  }
        public string? Description { get; set; }
        public DateTime Day_of_start { get; set; }
        public DateTime Day_of_end { get; set; }
    }
}
