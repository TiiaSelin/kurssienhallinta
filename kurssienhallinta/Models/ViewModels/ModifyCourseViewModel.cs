using kurssienhallinta.Models;

namespace kurssienhallinta.Models.ViewModels
{
    public class ModifyCourseViewModel
    {
        public Kurssi Kurssi { get; set; } = new();
        public List<Opettaja> Opettajat { get; set; } = new();
        public List<Tila> Tilat { get; set; } = new();
    }
}
