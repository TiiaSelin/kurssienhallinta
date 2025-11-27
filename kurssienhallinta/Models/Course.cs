using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace kurssienhallinta.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set;  }
        public string Description { get; set; } 
        public DateTime Day_of_start { get; set; }
        public DateTime Day_of_end { get; set; }
        public int? TeacherId { get; set; }  
        public int? RoomId { get; set; }


        [ForeignKey("TeacherId")]
        [ValidateNever]
        public Teacher? Teacher { get; set; }

        [ForeignKey("RoomId")]
        [ValidateNever]
        public Room? Room { get; set; }
        [ValidateNever]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        [ValidateNever]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }

}

