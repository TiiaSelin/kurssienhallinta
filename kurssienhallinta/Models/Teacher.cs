using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kurssienhallinta.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        [Required]
        public string Teacher_code { get; set; }
        [Required]
        public string First_name { get; set; } = null!;
        [Required]
        public string Last_name { get; set; } = null!;
        [Required]
        public string Subject { get; set; } = null!;

        // Navigation property - one teacher can have many courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}