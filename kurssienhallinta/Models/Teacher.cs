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

        public string FullName
        {
            get { return Teacher_code + ", " + First_name + " " + Last_name; }
        }

        // Navigation property - one teacher can have many courses

        [ValidateNever]
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}