using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace kurssienhallinta.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public string Room_code { get; set; }
        [ValidateNever]
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
