using kurssienhallinta.Models;

namespace kurssienhallinta.Models.ViewModels
{
    public class KurssinOpiskelijatViewModel
    {
        public List<Kurssi> Kurssit { get; set; } = new();
        public List<Opiskelija> Opiskelijat { get; set; } = new();
    }
}
