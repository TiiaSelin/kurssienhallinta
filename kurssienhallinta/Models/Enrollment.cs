using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kurssienhallinta.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }

        [ForeignKey("StudentId")]
        [ValidateNever]
        public Student Student { get; set; } = null!;
        
        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; } = null!;
    }
}
