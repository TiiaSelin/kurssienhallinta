namespace kurssienhallinta.Models
{

    public class Kirjautuminen
    {
        public int? enroltunnus { get; set; }
        public string? opiskelijatunnus { get; set; }
        public string? kurssitunnus { get; set; }
        public DateTime? enroldate { get; set; } = DateTime.Today;
        public Opiskelija? Opiskelija { get; set; }
        public Kurssi? Kurssi { get; set; }

    }

}