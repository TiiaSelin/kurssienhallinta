using System.ComponentModel.DataAnnotations;

namespace kurssienhallinta.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string Student_code { get; set; }
        [Required]
        public string F_Name { get; set; }
        [Required]
        public string L_Name { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public int Year { get; set; }

        public string FullName
        {
            get { return F_Name + " " + L_Name; }
        }
    }
}
