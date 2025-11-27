using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace kurssienhallinta.Models
{
    public class CourseSession
    {
        public int Id {get; set;}

        public int CourseId {get; set;}

        [ValidateNever] 
        public Course Course {get; set;}

        [Required]
        public DayOfWeek WeekDay {get; set;}
        [Required]
        public TimeSpan Time_of_start {get; set;}
        public TimeSpan Time_of_end {get; set;}
    }
}