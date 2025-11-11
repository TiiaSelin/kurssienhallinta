namespace kurssienhallinta.Models
{
    public class Kurssi
    {
        public string? kurssitunnus { get; set; }
        public string? kurssinimi { get; set; }
        public string? kurssikuvaus { get; set; }
        public DateTime? kurssialoituspaiva { get; set; }
        public DateTime? kurssilopetuspaiva { get; set; }
        public string? opettajatunnus { get; set; }
        public string? tilatunnus { get; set; }
    }
    
}