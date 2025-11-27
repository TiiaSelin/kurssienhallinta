using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace kurssienhallinta.Models
{
    public class CourseSession
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Column("WeekDay")]
        public string Weekday { get; set; }

        [Column("Time_of_start")]
        public TimeSpan Time_of_start { get; set; }

        [Column("Time_of_end")]
        public TimeSpan Time_of_end { get; set; }
    }
}
