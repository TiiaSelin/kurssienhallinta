using System.ComponentModel.DataAnnotations;

namespace kurssienhallinta.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Capacity { get; set; }
        [Required]
        public string Room_code { get; set; }
    }
}
