namespace kurssienhallinta.Models
{
    public class KurssinOpiskelijat
    {
        public Kurssi Kurssi { get; set; } = null!;
        public List<Opiskelija> Opiskelijat { get; set; } = new List<Opiskelija>();
    }
}
