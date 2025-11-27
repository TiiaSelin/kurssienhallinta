using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace kurssienhallinta.Models
{
    public class CourseSession
    {
        public int Id { get; set; }
        public string CourseId { get; set; }
        public string Weekday { get; set; }
         public TimeSpan Time_of_Start { get; set; }
        public TimeSpan Time_of_End { get; set; }
       
    }
}
