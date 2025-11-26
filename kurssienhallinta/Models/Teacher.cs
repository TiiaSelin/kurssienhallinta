using System.ComponentModel.DataAnnotations;
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
    }

}