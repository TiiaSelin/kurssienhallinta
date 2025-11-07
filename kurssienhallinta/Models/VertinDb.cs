namespace VertinDb.Models
{
    public class Kurssi
    {
        public string? Kurssitunnus { get; set; }
        public string? Kurssinimi { get; set; }
        public string? Kurssikuvaus { get; set; }
        public DateTime? Kurssialoituspaiva { get; set; }
        public DateTime? Kurssilopetuspaiva { get; set; }
        public string? Opettajatunnus { get; set; }
        public string? Tilatunnus { get; set; }
    }
}