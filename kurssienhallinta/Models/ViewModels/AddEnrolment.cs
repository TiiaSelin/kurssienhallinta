namespace kurssienhallinta.Models.ViewModels
{
    public class AddEnrolmentViewModel
    {
        public Kirjautuminen Kirjautuminen { get; set; } = new();
        public List<Kurssi> Kurssit { get; set; } = new();
        public List<Opiskelija> Opiskelijat { get; set; } = new();
    }
}