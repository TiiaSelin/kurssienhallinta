using System.ComponentModel.DataAnnotations;
namespace kurssienhallinta.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Teacher_code { get; set; }
        public string First_name { get; set; } = null!;
        public string Last_name { get; set; } = null!;
        public string Subject { get; set; } = null!;
    }

}