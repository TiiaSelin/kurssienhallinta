namespace kurssienhallinta.Models
{
    public class Opiskelija
    {
        public string? opiskelijatunnus { get; set; }
        public string? opiskelijaetunimi { get; set; }
        public string? opiskelijasukunimi { get; set; }
        public DateTime? syntymaaika { get; set; }
        public int? vuosikurssi { get; set; }
    }


}