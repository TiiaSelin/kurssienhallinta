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
        
        // Navigation properties - EF Core uses these for joins
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;
        
        [ForeignKey("CourseId")]
        public Course Course { get; set; } = null!;
    }
}